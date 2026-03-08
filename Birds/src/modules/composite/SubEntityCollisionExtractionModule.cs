using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class SubEntityCollisionExtractionModule : ModuleBase
{
  protected override void Update(GameTime gameTime)
  {
    container.Collisions.Clear();

    foreach (var child in container.Entities)
    {
      foreach (var collision in child.Collisions)
      {
        if (!container.Collisions.Contains(collision))
        {
          container.Collisions.Add(collision);
        }
        if (!collision.Collisions.Contains(container))
        {
          collision.Collisions.Add(container);
        }
      }
      child.Collisions.Clear();
    }
  }
}