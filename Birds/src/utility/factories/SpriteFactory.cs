using System;
using System.Collections.Generic;
using Birds.src.utility;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.factories;

public static class SpriteFactory
{
  public static Stack<Sprite> availableSprites = new();
  public static Texture2D[] textures;

  private static readonly Dictionary<ID_SPRITE, Action<Sprite>> SpecialConfigMap = new()
    {
        {
            ID_SPRITE.BACKGROUND_WHITE, s => {
                s.Alpha = 0.8f;
                s.Scale = 1.2f * Math.Max(Game1.ScreenWidth / s.Width, Game1.ScreenHeight / s.Height);
            }
        },
        {
            ID_SPRITE.BACKGROUND_GRAY, s => {
                s.Scale = 1.2f * Math.Max(Game1.ScreenWidth / s.Width, Game1.ScreenHeight / s.Height);
            }
        },
        {
            ID_SPRITE.CLOUD, s => {
                s.Alpha = 0.8f;
            }
        }
    };

  public static Sprite GetSprite(ID_SPRITE id, Vector2 position, float scale = 1, float alpha = 1)
  {
    Sprite s = availableSprites.Count > 0 ? availableSprites.Pop() : new Sprite();
    s.Position = position;
    s.Scale = scale;
    s.Alpha = alpha;
    if ((int)id < textures.Length)
    {
      s.Texture = textures[(int)id];
    }
    if (SpecialConfigMap.TryGetValue(id, out var applySpecialLogic))
    {
      applySpecialLogic(s);
    }

    return s;
  }
}