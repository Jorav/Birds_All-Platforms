using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;
public class GroupPositionModule : ModuleBase
{
  public Vector2 Position { get; set; }

  protected override void ConfigurePropertySync()
  {
    ReadWriteSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    Vector2 sum = Vector2.Zero;
    foreach (IEntity entity in container.Entities)
    {
      sum += entity.Position.Value;
    }
    if (container.Entities.Count > 0)
    {
      Position = sum / container.Entities.Count;
    }
  }
}

