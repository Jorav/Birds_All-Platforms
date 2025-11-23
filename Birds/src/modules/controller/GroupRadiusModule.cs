using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller;

public class GroupRadiusModule : ModuleBase
{
  public float Radius { get; set; }
  public Vector2 Position { get; set; }

  protected override void ConfigurePropertySync()
  {
    WriteSync(() => Radius, container.Radius);
    ReadSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    UpdateRadius();
  }

  private void UpdateRadius()
  {
    float largestDistance = 0;

    foreach (IEntity entity in container.Entities)
    {
      float distance = Vector2.Distance(entity.Position, Position) + entity.Radius;
      if (distance > largestDistance)
        largestDistance = distance;
    }

    Radius = largestDistance;
  }
}

