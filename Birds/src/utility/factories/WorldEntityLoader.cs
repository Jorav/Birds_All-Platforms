using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.modules.entity;
using Birds.src.modules.entity.collision_handling;
using Birds.src.collision.bounding_areas;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Birds.src.utility.factories.model;
using System.Linq;

namespace Birds.src.utility.factories;

public class WorldEntityLoader
{
  private const string CONFIG_PATH = "src/WorldEntities.json";

  private static WorldEntityConfiguration _defaultConfig;
  private static Dictionary<ID_ENTITY, WorldEntityConfiguration> _entityConfigs = new();
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
    string json = LoadConfigFile();
    var configFile = JsonSerializer.Deserialize<WorldEntityConfigurationFile>(json, _jsonOptions);

    _defaultConfig = configFile.Default;
    LoadEntityConfigurations(configFile);
  }

  private static string LoadConfigFile()
  {
    using (var stream = TitleContainer.OpenStream(CONFIG_PATH))
    using (var reader = new StreamReader(stream))
    {
      return reader.ReadToEnd();
    }
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

  private static WorldEntityConfiguration MergeWithDefault(WorldEntityConfiguration entityConfig)
  {
    var merged = new WorldEntityConfiguration
    {
      Properties = new Dictionary<string, object>(_defaultConfig.Properties),
      Modules = entityConfig.Modules?.Count > 0
        ? new List<ModuleConfigurationEntry>(entityConfig.Modules)
        : new List<ModuleConfigurationEntry>(_defaultConfig.Modules)
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

  public static void ApplyConfiguration(IEntity entity, ID_ENTITY entityId, bool isPartOfComposite = false)
  {
    if (!_entityConfigs.TryGetValue(entityId, out var config))
    {
      throw new ArgumentException($"No configuration found for entity type: {entityId}");
    }

    ApplyProperties(entity, config.Properties);
    Sprite sprite = CreateSpriteIfNeeded(entity, entityId);
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

  private static Sprite CreateSpriteIfNeeded(IEntity entity, ID_ENTITY entityId)
  {
    if (WorldEntityFactory.EntityToSpriteMap.TryGetValue(entityId, out var spriteId))
    {
      var sprite = SpriteFactory.GetSprite(spriteId, entity.Position.Value, entity.Scale.Value);
      entity.Width.Value = sprite.Width;
      entity.Height.Value = sprite.Height;
      return sprite;
    }
    return null;
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
        if (moduleConfig.Parameters.TryGetValue("Links", out var linksObj) && linksObj is JsonElement linksJson)
        {
          var linkConfigs = JsonSerializer.Deserialize<List<LinkConfiguration>>(linksJson.GetRawText(), _jsonOptions);
          linkModule.SetLinkConfigurations(linkConfigs);
        }
        entity.AddModule(linkModule);
        break;

      case ID_MODULE.DrawModule:
        if (sprite != null)
          entity.AddModule(new DrawModule(sprite));
        break;

      default:
        throw new NotImplementedException($"Module type '{moduleConfig.Type}' is not implemented");
    }
  }

  public static bool HasModule(ID_ENTITY entityId, ID_MODULE moduleType)
  {
    if (!_entityConfigs.TryGetValue(entityId, out var config))
      return false;

    return config.Modules.Any(m => m.Type == moduleType);
  }
}