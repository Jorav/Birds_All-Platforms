using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.shared.position;

public class GroupGeometricPositionModule : ModuleBase
{
  public Vector2 Position { get; set; }

  protected override void ConfigurePropertySync()
  {
    ReadWriteSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    Position = CalculateGeographicCenter(container.Entities);
  }

  public static Vector2 CalculateGeographicCenter(IEnumerable<IEntity> entities)
  {
    if (entities == null || !entities.Any())
      return Vector2.Zero;

    float minX = float.MaxValue;
    float maxX = float.MinValue;
    float minY = float.MaxValue;
    float maxY = float.MinValue;

    foreach (IEntity entity in entities)
    {
      Vector2 pos = entity.Position.Value;

      if (pos.X < minX) minX = pos.X;
      if (pos.X > maxX) maxX = pos.X;
      if (pos.Y < minY) minY = pos.Y;
      if (pos.Y > maxY) maxY = pos.Y;
    }

    return new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
  }
}