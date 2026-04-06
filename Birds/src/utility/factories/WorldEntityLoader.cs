using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.modules.entity;
using Birds.src.collision.bounding_areas;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using Birds.src.utility.factories.model.entity;
using Birds.src.network;

namespace Birds.src.utility.factories;

public class WorldEntityLoader
{
  private const string CONFIG_PATH = "src/WorldEntities.json";
  public const string PARAM_LINKS = "Links";
  private static WorldEntityConfiguration _defaultConfig;
  private static Dictionary<ID_ENTITY, WorldEntityConfiguration> _entityConfigs = new();
  public static List<ID_ENTITY> Hulls { get; private set; } = new();
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    Converters =
    {
      new System.Text.Json.Serialization.JsonStringEnumConverter()
    }
  };

  public static void Initialize()
  {
    string json = File.ReadAllText(CONFIG_PATH);
    var configFile = JsonSerializer.Deserialize<WorldEntityConfigurationFile>(json, _jsonOptions);
    _defaultConfig = configFile.Default;
    LoadEntityConfigurations(configFile);
    IdentifyHulls();
  }

  private static void LoadEntityConfigurations(WorldEntityConfigurationFile configFile)
  {
    foreach (ID_ENTITY entityId in Enum.GetValues(typeof(ID_ENTITY)))
    {
      string entityName = entityId.ToString();
      if (configFile.Entities.TryGetValue(entityName, out var config))
      {
        _entityConfigs[entityId] = MergeWithDefault(config);
      }
      else
      {
        _entityConfigs[entityId] = _defaultConfig;
      }
    }
  }

  private static void IdentifyHulls()
  {
    Hulls.Clear();
    foreach (var kvp in _entityConfigs)
    {
      if (kvp.Key == ID_ENTITY.FILLER) continue;
      var linkModule = kvp.Value.Modules?.FirstOrDefault(m => m.Type == ID_MODULE.LinkModule);
      if (linkModule != null && linkModule.Parameters.TryGetValue(PARAM_LINKS, out var linksObj) && linksObj is JsonElement linksJson)
      {
        var links = JsonSerializer.Deserialize<List<LinkConfiguration>>(linksJson.GetRawText(), _jsonOptions);
        if (links != null && links.Count > 1)
        {
          Hulls.Add(kvp.Key);
        }
      }
    }
  }

  private static WorldEntityConfiguration MergeWithDefault(WorldEntityConfiguration entityConfig)
  {
    var merged = new WorldEntityConfiguration
    {
      Properties = new Dictionary<string, object>(_defaultConfig.Properties),
      Modules = entityConfig.Modules?.Count > 0
        ? (entityConfig.Modules)
        : (_defaultConfig.Modules)
    };
    if (entityConfig.Properties != null)
    {
      foreach (var kvp in entityConfig.Properties)
      {
        merged.Properties[kvp.Key] = kvp.Value;
      }
    }
    return merged;
  }

  public static void ApplyConfiguration(IEntity entity, ID_ENTITY entityId, bool isPartOfComposite = false, ID_SPRITE spriteId = ID_SPRITE.FILLER)
  {
    if (!_entityConfigs.TryGetValue(entityId, out var config))
    {
      throw new ArgumentException($"No configuration found for entity type: {entityId}");
    }
    ApplyProperties(entity, config.Properties);

    Sprite sprite = null;

    if (!RuntimeContext.IsServer)
    {
      ID_SPRITE finalSpriteId = (spriteId == ID_SPRITE.FILLER && WorldEntityFactory.EntityToSpriteMap.TryGetValue(entityId, out var mappedId))
          ? mappedId
          : spriteId;
      sprite = SpriteFactory.GetSprite(finalSpriteId, entity.Position.Value, entity.Scale.Value);
      if (entityId == ID_ENTITY.FILLER)
      {
        sprite.Alpha = 0.4f;
        sprite.Color = Color.Lime;
      }
    }

    ID_SPRITE pathSpriteId = (spriteId == ID_SPRITE.FILLER && WorldEntityFactory.EntityToSpriteMap.TryGetValue(entityId, out var pathMappedId))
        ? pathMappedId
        : spriteId;
    string pngPath = SpriteFactory.GetSpritePath(pathSpriteId);
    var (width, height) = SizeReader.GetPngSize(pngPath);
    entity.Width.Value = width;
    entity.Height.Value = height;

    ApplyModules(entity, config.Modules, isPartOfComposite, sprite);
  }

  private static void ApplyProperties(IEntity entity, Dictionary<string, object> properties)
  {
    foreach (var kvp in properties)
    {
      var property = entity.GetType().GetProperty(kvp.Key);
      if (property == null) continue;
      if (property.PropertyType.IsGenericType &&
          property.PropertyType.GetGenericTypeDefinition() == typeof(SyncedProperty<>))
      {
        var syncedProp = property.GetValue(entity);
        var valueProperty = property.PropertyType.GetProperty("Value");
        var targetType = valueProperty.PropertyType;
        object convertedValue;
        if (kvp.Value is JsonElement jsonElement)
        {
          convertedValue = JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType, _jsonOptions);
        }
        else
        {
          convertedValue = Convert.ChangeType(kvp.Value, targetType);
        }
        valueProperty.SetValue(syncedProp, convertedValue);
      }
    }
  }

  public static bool HasModule(ID_ENTITY entityId, ID_MODULE moduleType)
  {
    if (_entityConfigs.TryGetValue(entityId, out var config))
    {
      return config.Modules.Any(m => m.Type == moduleType);
    }
    return false;
  }

  private static void ApplyModules(IEntity entity, List<ModuleConfigurationEntry> modules, bool isPartOfComposite, Sprite sprite)
  {
    entity.ClearModules();
    foreach (var moduleConfig in modules)
    {
      AddModule(entity, moduleConfig, isPartOfComposite, sprite);
    }
  }

  private static void AddModule(IEntity entity, ModuleConfigurationEntry moduleConfig, bool isPartOfComposite, Sprite sprite)
  {
    switch (moduleConfig.Type)
    {
      case ID_MODULE.CollisionHandlerModule:
        entity.AddModule(WorldEntityFactory.GetCollisionHandler(isPartOfComposite));
        break;
      case ID_MODULE.MovementModule:
        if (!isPartOfComposite)
          entity.AddModule(new MovementModule());
        break;
      case ID_MODULE.RotationModule:
        if (!isPartOfComposite)
          entity.AddModule(new RotationModule());
        break;
      case ID_MODULE.RadiusModule:
        entity.AddModule(new RadiusModule());
        break;
      case ID_MODULE.CollisionDetectionModule:
        entity.AddModule(new CollisionDetectionModule(
          BoundingAreaFactory.GetOBB(
            entity.Position.Value,
            entity.Rotation.Value,
            (int)entity.Width.Value,
            (int)entity.Height.Value)
        ));
        break;
      case ID_MODULE.LinkModule:
        var linkModule = new LinkModule();
        if (moduleConfig.Parameters.TryGetValue(PARAM_LINKS, out var linksObj) && linksObj is JsonElement linksJson)
        {
          var linkConfigs = JsonSerializer.Deserialize<List<LinkConfiguration>>(linksJson.GetRawText(), _jsonOptions);
          linkModule.SetLinkConfigurations(linkConfigs);
        }
        entity.AddModule(linkModule);
        break;
      case ID_MODULE.DrawModule:
        if (!RuntimeContext.IsServer && sprite != null)
          entity.AddModule(new DrawModule(sprite));
        break;
    }
  }
}