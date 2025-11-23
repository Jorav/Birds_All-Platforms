using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.entity;

public class CollisionsClearer : ModuleBase
{
  protected override void Update(GameTime gameTime)
  {
    container.Collisions.Clear();
  }
}

