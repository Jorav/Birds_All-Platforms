using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;

public class LinkConfiguration
{
  public float Angle { get; set; }
  public float Distance { get; set; }

  public Vector2 GetOffset(float spriteWidth)
  {
    float angleRadians = MathHelper.ToRadians(Angle);
    float actualDistance = Distance * spriteWidth;

    return new Vector2(
      (float)Math.Cos(angleRadians) * actualDistance,
      (float)Math.Sin(angleRadians) * actualDistance
    );
  }
}