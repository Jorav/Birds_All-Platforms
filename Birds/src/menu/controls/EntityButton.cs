using Birds.src.visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Birds.src.menu.controls;

public class EntityButton : Button
{
  public ISprite entitySprite { get; private set; }
  private float hullFactor = 1f;
  public bool IsClicked { get; set; }

  public override Vector2 Position
  {
    get => position;
    set
    {
      position = value;
      sprite.Position = value;

      entitySprite.Position = value + new Vector2((sprite.Width * scale) / 2, (sprite.Height * scale) / 2);
    }
  }

  public override float Scale
  {
    get => scale;
    set
    {
      scale = value;
      sprite.Scale = value;
      entitySprite.Scale = value * hullFactor;
      Position = position;
    }
  }

  public EntityButton(ISprite entitySprite, ISprite backgroundSprite, bool autoFit = true, SpriteFont font = null)
      : base(backgroundSprite, font)
  {
    this.entitySprite = entitySprite;

    if (autoFit)
    {
      CalculateHullFactor(0.8f);
    }
  }

  private void CalculateHullFactor(float paddingFactor)
  {
    float ratioX = (float)sprite.Width / entitySprite.Width;
    float ratioY = (float)sprite.Height / entitySprite.Height;

    hullFactor = Math.Min(ratioX, ratioY) * paddingFactor;
  }

  public override void Draw(SpriteBatch spritebatch)
  {
    bool previousIsHovering = isHovering;
    if (IsClicked)
      isHovering = true;

    base.Draw(spritebatch);
    entitySprite.Draw(spritebatch);

    isHovering = previousIsHovering;
  }
}
