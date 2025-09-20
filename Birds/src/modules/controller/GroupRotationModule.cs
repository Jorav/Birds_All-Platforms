using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller;

public class GroupRotationModule : RotationModuleBase
{
  protected override void Update(GameTime gameTime)
  {
  }
  public override void RotateTo(Vector2 position)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.RotateTo(position);
    }
  }
}

