using Birds.src.utility;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Birds.src.factories;

public static class SpriteFactory
{
  public static readonly ThreadLocal<Stack<Sprite>> availableSprites =
      new(() => new Stack<Sprite>());
  public static Texture2D[] textures;

  private static readonly Dictionary<ID_SPRITE, string> _spritePaths = new()
    {
        { ID_SPRITE.HULL_RECTANGULAR_BAD, "parts/HULL_RECTANGULAR_BAD" },
        { ID_SPRITE.HULL_RECTANGULAR, "parts/HULL_RECTANGULAR" },
        { ID_SPRITE.HULL_RECTANGULAR_GOOD, "parts/HULL_RECTANGULAR_GOOD" },
        { ID_SPRITE.HULL_CIRCULAR, "parts/HULL_CIRCULAR" },
        { ID_SPRITE.HULL_LINK, "parts/HULL_LINK" },
        { ID_SPRITE.ENGINE_BAD, "parts/ENGINE_BAD" },
        { ID_SPRITE.ENGINE, "parts/ENGINE" },
        { ID_SPRITE.ENGINE_GOOD, "parts/ENGINE_GOOD" },
        { ID_SPRITE.GUN, "parts/GUN" },
        { ID_SPRITE.FILLER, "parts/PART_EMPTY_DIRECTED" },
        { ID_SPRITE.SPIKE, "parts/SPIKE" },
        { ID_SPRITE.HULL_THIN, "parts/HULL_THIN" },
        { ID_SPRITE.CLOUD, "background/CLOUD" },
        { ID_SPRITE.SUN, "background/SUN" },
        { ID_SPRITE.BACKGROUND_WHITE, "background/WHITE_SMALL" },
        { ID_SPRITE.BACKGROUND_GRAY, "background/GRAY" },
        { ID_SPRITE.BUTTON, "menu/BUTTON" },
        { ID_SPRITE.ENTITY_BUTTON, "menu/BUTTON_ENTITY" },
        { ID_SPRITE.ENTITY_BUTTON_MENU, "menu/BUTTON_ENTITY_MENU" },
        { ID_SPRITE.DELETE_X, "menu/DELETE_X" },
        { ID_SPRITE.DELETE_BUTTON, "menu/DELETE_BUTTON" },
        { ID_SPRITE.SAVE, "menu/SAVE" }
    };

  public static void LoadTextures(ContentManager content)
  {
    textures = new Texture2D[Enum.GetNames(typeof(ID_SPRITE)).Length];

    foreach (var kvp in _spritePaths)
    {
      textures[(int)kvp.Key] = content.Load<Texture2D>(kvp.Value);
    }
  }

  public static string GetSpritePath(ID_SPRITE id)
  {
    if (_spritePaths.TryGetValue(id, out var path))
    {
      return $"Content/{path}.png";
    }
    return "Content/parts/PART_EMPTY_DIRECTED.png";
  }

  public static Sprite GetSprite(ID_SPRITE id, Vector2 position, float scale = 1, float alphaOverride = -1)
  {
    var stack = availableSprites.Value;
    Sprite s = stack.Count > 0 ? stack.Pop() : new Sprite();
    if ((int)id < textures.Length)
    {
      s.Texture = textures[(int)id];
    }
    SpriteLoader.ApplyConfiguration(s, id);
    s.Scale = scale;
    if (alphaOverride >= 0)
    {
      s.Alpha = alphaOverride;
    }
    s.Position = position;
    return s;
  }

  public static float GetBackgroundScale(ID_SPRITE id)
  {
    var texture = textures[(int)id];
    return 1.2f * Math.Max((float)Game1.ScreenWidth / texture.Width, (float)Game1.ScreenHeight / texture.Height);
  }
}
