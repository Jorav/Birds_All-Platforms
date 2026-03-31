using Birds.src.visual;
using Birds.src.factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Birds.src.utility;

namespace Birds.src.menu.controls;

public class EntityButton : Button
{
  public ISprite sprite { get; private set; }
  private float hullFactor = 1f;
  public bool IsClicked { get; set; }
  public bool IsDeleteMode { get; set; }

  public override Vector2 Position
  {
    get => position;
    set
    {
      position = value;
      base.sprite.Origin = Vector2.Zero;
      base.sprite.Position = value;

      if (sprite != null)
      {
        sprite.Position = value + new Vector2((base.sprite.Width * scale) / 2, (base.sprite.Height * scale) / 2);
      }
    }
  }

  public override float Scale
  {
    get => scale;
    set
    {
      scale = value;
      base.sprite.Scale = value;
      if (sprite != null) sprite.Scale = value * hullFactor;
      Position = position;
    }
  }

  public EntityButton(ISprite entitySprite, ISprite backgroundSprite, bool autoFit = true, SpriteFont font = null)
      : base(backgroundSprite, font)
  {
    this.sprite = entitySprite;
    if (autoFit) CalculateHullFactor(0.8f);
    this.Position = position;
  }

  private void CalculateHullFactor(float paddingFactor)
  {
    if (sprite == null) return;
    float ratioX = (float)base.sprite.Width / sprite.Width;
    float ratioY = (float)base.sprite.Height / sprite.Height;
    hullFactor = Math.Min(ratioX, ratioY) * paddingFactor;
  }

  public override void Draw(SpriteBatch spritebatch)
  {
    bool previousIsHovering = isHovering;
    if (IsClicked) isHovering = true;

    base.Draw(spritebatch);
    sprite?.Draw(spritebatch);

    if (IsDeleteMode)
    {
      Vector2 center = Position + new Vector2((base.sprite.Width * scale) / 2, (base.sprite.Height * scale) / 2);
      var deleteSprite = SpriteFactory.GetSprite(ID_SPRITE.DELETE, center, scale);
      deleteSprite.Draw(spritebatch);
    }

    isHovering = previousIsHovering;
  }
}