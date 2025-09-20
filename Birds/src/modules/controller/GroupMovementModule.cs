using Microsoft.Xna.Framework;
using Birds.src.events;
using Birds.src.containers.entity;

namespace Birds.src.modules.controller;
public class GroupMovementModule : ModuleBase, IMovementModule
{
  protected override void Update(GameTime gameTime)
  {
  }

  public void AccelerateTo(Vector2 position, float thrust)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.AccelerateTo(position, thrust);
    }
  }

  public void Accelerate(Vector2 directionalVector, float thrust)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.Accelerate(directionalVector, thrust);
    }
  }

  public void Accelerate(Vector2 directionalVector)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.Accelerate(directionalVector);
    }
  }
}

