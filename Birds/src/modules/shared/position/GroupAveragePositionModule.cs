using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.modules.composite;

public class GroupAveragePositionModule : ModuleBase
{
  public Vector2 Position { get; set; }

  protected override void ConfigurePropertySync()
  {
    ReadWriteSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    Position = CalculateAverageCenter(container.Entities);
  }

  public static Vector2 CalculateAverageCenter(IEnumerable<IEntity> entities)
  {
    Vector2 sum = Vector2.Zero;
    int count = 0;
    foreach (IEntity entity in entities)
    {
      sum += entity.Position.Value;
      count++;
    }
    return count > 0 ? sum / count : Vector2.Zero;
  }
}