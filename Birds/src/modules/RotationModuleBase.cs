using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules;

public abstract class RotationModuleBase : ModuleBase
{
  public abstract void RotateTo(Vector2 position);

  public static float CalculateRotation(Vector2 positionLookedAt, Vector2 currentPosition)
  {
    Vector2 position = positionLookedAt - currentPosition;
    if (position.X >= 0)
      return (float)Math.Atan(position.Y / position.X);
    else
      return (float)Math.Atan(position.Y / position.X) - MathHelper.ToRadians(180);
  }
}

