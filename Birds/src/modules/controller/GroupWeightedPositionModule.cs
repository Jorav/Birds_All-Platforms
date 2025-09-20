using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller;
public class GroupWeightedPositionModule : ModuleBase
{
  public Vector2 Position { get; set; }
  public float Mass { get; set; }

  protected override void ConfigurePropertySync()
  {
    WriteSync(() => Position, container.Position);
    ReadSync(() => Mass, container.Mass);
  }

  protected override void Update(GameTime gameTime)
  {
    Vector2 sum = Vector2.Zero;
    float weight = 0;
    foreach (IEntity entity in container.Entities)
    {
      weight += entity.Mass;
      sum += entity.Position.Value * entity.Mass.Value;
    }
    if (weight > 0)
    {
      Position = sum / weight;
    }
  }
}

