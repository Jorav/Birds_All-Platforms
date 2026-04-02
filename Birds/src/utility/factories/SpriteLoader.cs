using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Birds.src.utility;
using Birds.src.utility.factories.model.sprite;
using Birds.src.visual;

namespace Birds.src.factories;

public static class SpriteLoader
{
  private const string CONFIG_PATH = "src/Sprites.json";

  private static SpriteConfiguration _defaultConfig;
  private static Dictionary<ID_SPRITE, SpriteConfiguration> _spriteConfigs = new();

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
    var configFile = JsonSerializer.Deserialize<SpriteConfigurationFile>(json, _jsonOptions);

    _defaultConfig = configFile.Default ?? new SpriteConfiguration();
    LoadSpriteConfigurations(configFile);
  }

  private static void LoadSpriteConfigurations(SpriteConfigurationFile configFile)
  {
    foreach (ID_SPRITE spriteId in Enum.GetValues(typeof(ID_SPRITE)))
    {
      string spriteName = spriteId.ToString();

      if (configFile.Sprites.TryGetValue(spriteName, out var config))
      {
        _spriteConfigs[spriteId] = MergeWithDefault(config);
      }
      else
      {
        _spriteConfigs[spriteId] = _defaultConfig;
      }
    }
  }

  private static SpriteConfiguration MergeWithDefault(SpriteConfiguration spriteConfig)
  {
    return new SpriteConfiguration
    {
      Alpha = spriteConfig.Alpha ?? _defaultConfig.Alpha,
      Color = spriteConfig.Color ?? _defaultConfig.Color,
      Visible = spriteConfig.Visible
    };
  }

  public static void ApplyConfiguration(Sprite sprite, ID_SPRITE spriteId)
  {
    if (!_spriteConfigs.TryGetValue(spriteId, out var config))
    {
      config = _defaultConfig;
    }

    sprite.Alpha = config.Alpha ?? _defaultConfig?.Alpha ?? 1f;
    sprite.isVisible = config.Visible;

    if (config.Color != null)
    {
      sprite.Color = config.Color.ToColor();
    }
  }
}