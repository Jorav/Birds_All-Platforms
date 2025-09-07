using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;

public class RotationModule : ModuleBase, IRotationModule
{
  public virtual float Rotation { get; set; }
  protected float rotation;
  public virtual Vector2 Position { get; set; }


  protected override void ConfigurePropertySync()
  {
    ReadWriteSync(() => Rotation, container.Rotation);
    ReadSync(() => Position, container.Position);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    Rotation = 0;
  }

  protected override void Update(GameTime gameTime)
  {
  }

  public static float CalculateRotation(Vector2 positionLookedAt, Vector2 currentPosition)
  {
    Vector2 position = positionLookedAt - currentPosition;
    if (position.X >= 0)
      return (float)Math.Atan(position.Y / position.X);
    else
      return (float)Math.Atan(position.Y / position.X) - MathHelper.ToRadians(180);
  }

  public virtual void RotateTo(Vector2 position)
  {
    Rotation = CalculateRotation(position, Position);
  }

  public override object Clone()
  {
    var cloned = new RotationModule();
    cloned.Rotation = this.Rotation;
    return cloned;
  }
}

