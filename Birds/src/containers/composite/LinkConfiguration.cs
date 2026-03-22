using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;

public class LinkConfiguration
{
  public float Angle { get; set; }
  public float Distance { get; set; }

  public Vector2 GetOffset(float spriteWidth, float spriteHeight)
  {
    float angleRadians = MathHelper.ToRadians(Angle);
    float cosAngle = (float)Math.Abs(Math.Cos(angleRadians));
    float sinAngle = (float)Math.Abs(Math.Sin(angleRadians));

    float effectiveDimension = spriteWidth * cosAngle + spriteHeight * sinAngle;
    float actualDistance = Distance * effectiveDimension;

    return new Vector2(
      (float)Math.Cos(angleRadians) * actualDistance,
      (float)Math.Sin(angleRadians) * actualDistance
    );
  }
}