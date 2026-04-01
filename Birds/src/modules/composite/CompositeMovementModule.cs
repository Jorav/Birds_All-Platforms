using Birds.src.containers.entity;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class CompositeMovementModule : MovementModule
{
  protected override void Move(Vector2 distance)
  {
    if (_manualMoveTarget.HasValue)
    {
      foreach (IEntity entity in container.Entities)
      {
        entity.Position.Value += distance;
        entity.Velocity.Value = this.Velocity;
      }
      this.Velocity = Vector2.Zero;
    }
    else
    {
      Vector2 compositeVelocity = distance / ((float)Game1.timeStep * 60f);

      foreach (IEntity entity in container.Entities)
      {
        entity.Velocity.Value += compositeVelocity;
        entity.Position.Value += entity.Velocity.Value * (float)Game1.timeStep * 60f;
      }
    }

    container.Position.Value += distance;
  }
}