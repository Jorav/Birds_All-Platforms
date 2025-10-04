using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class CompositeMovementModule : MovementModule
{
  protected override void Move(Vector2 distance)
  {
    foreach(IEntity entity in container.Entities)
    {
      entity.Velocity.Value += distance;
      entity.Position.Value += entity.Velocity.Value;
    }
    Position += distance;
  }
}

