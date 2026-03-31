using Birds.src.containers.entity;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class CompositeMovementModule : MovementModule
{
  private bool _isManualMove;

  public override void PerformManualMove(Vector2 targetPosition)
  {
    _isManualMove = true;
    base.PerformManualMove(targetPosition);
    _isManualMove = false;
  }

  protected override void Move(Vector2 distance)
  {
    if (_isManualMove)
    {
      foreach (IEntity entity in container.Entities)
      {
        entity.Position.Value += distance;
        entity.Velocity.Value = this.Velocity;
      }
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