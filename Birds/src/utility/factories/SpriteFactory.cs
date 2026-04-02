using Birds.src.utility;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Birds.src.factories;

public static class SpriteFactory
{
  public static Stack<Sprite> availableSprites = new();
  public static Texture2D[] textures;

  public static Sprite GetSprite(ID_SPRITE id, Vector2 position, float scale = 1, float alphaOverride = -1)
  {
    Sprite s = availableSprites.Count > 0 ? availableSprites.Pop() : new Sprite();
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