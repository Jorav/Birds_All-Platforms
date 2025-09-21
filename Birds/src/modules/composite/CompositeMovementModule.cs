using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class CompositeMovementModule : MovementModule
{
  public override void UpdateModule(GameTime gameTime)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.Velocity.Value = Vector2.Zero;
    }
    base.UpdateModule(gameTime);
  }
  protected override void Move(Vector2 distance)
  {
    foreach(IEntity entity in container.Entities)
    {
      entity.Velocity.Value += distance;
    }
  }
}

