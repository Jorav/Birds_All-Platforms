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
  public static Texture2D[] textures;// = new Texture2D[100];
  public static Sprite GetSprite(ID_SPRITE id, Vector2 position, float scale = 1, float alpha = 1)
  {
    Sprite s;
    if (availableSprites.Count > 0)
    {
      s = availableSprites.Pop();
      s.Position = position;
      s.Scale = scale;
      s.Texture = textures[(int)id];
    }
    else
    {
      Vector2 defaultPosition = Vector2.Zero;
      s = new Sprite();
    }
    //set stats
    s.Position = position;
    s.Scale = scale;
    s.Alpha = alpha;
    switch (id)
    {
      #region parts
      case ID_SPRITE.HULL_RECTANGULAR: s.Texture = textures[(int)ID_SPRITE.HULL_RECTANGULAR]; break;
      case ID_SPRITE.HULL_CIRCULAR: s.Texture = textures[(int)ID_SPRITE.HULL_CIRCULAR]; break;
      case ID_SPRITE.HULL_LINK: s.Texture = textures[(int)ID_SPRITE.HULL_LINK]; break;
      case ID_SPRITE.ENGINE: s.Texture = textures[(int)ID_SPRITE.ENGINE]; break;
      case ID_SPRITE.GUN: s.Texture = textures[(int)ID_SPRITE.GUN]; break;
      case ID_SPRITE.SPIKE: s.Texture = textures[(int)ID_SPRITE.SPIKE]; break;
      case ID_SPRITE.PART_EMPTY: s.Texture = textures[(int)ID_SPRITE.PART_EMPTY]; break;
      #endregion
      #region background
      case ID_SPRITE.CLOUD: s.Texture = textures[(int)ID_SPRITE.CLOUD]; break;
      case ID_SPRITE.SUN: s.Texture = textures[(int)ID_SPRITE.SUN]; break;
      case ID_SPRITE.BACKGROUND_WHITE: s.Texture = textures[(int)ID_SPRITE.BACKGROUND_WHITE]; s.Alpha = 0.8f; s.Scale = 1.2f * Math.Max(Game1.ScreenWidth / s.Width, Game1.ScreenHeight / s.Height); break;
      case ID_SPRITE.BACKGROUND_GRAY: s.Texture = textures[(int)ID_SPRITE.BACKGROUND_GRAY]; s.Scale = 1.2f * Math.Max(Game1.ScreenWidth / s.Width, Game1.ScreenHeight / s.Height); break;
      #endregion
      #region menu
      case ID_SPRITE.BUTTON: s.Texture = textures[(int)ID_SPRITE.BUTTON]; break;
      case ID_SPRITE.BUTTON_ENTITY: s.Texture = textures[(int)ID_SPRITE.BUTTON_ENTITY]; break;
      #endregion


      default:
        throw new NotImplementedException();
    }
    return s;
  }

  public static Sprite GetSprite(ID_ENTITY id, Vector2 position, float scale = 1)
  {

    Vector2 defaultPosition = Vector2.Zero;
    switch (id)
    {
      case ID_ENTITY.DEFAULT: return GetSprite(ID_SPRITE.HULL_RECTANGULAR, position, scale);
      #region background
      case ID_ENTITY.CLOUD: return GetSprite(ID_SPRITE.CLOUD, position, scale);
      case ID_ENTITY.SUN: return GetSprite(ID_SPRITE.SUN, position, scale);
      #endregion

      default:
        throw new NotImplementedException();
    }
  }
}
