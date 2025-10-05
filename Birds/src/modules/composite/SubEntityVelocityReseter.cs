using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

internal class SubEntityVelocityReseter : ModuleBase
{
  protected override void Update(GameTime gameTime)
  {
    foreach(var entity in container.Entities)
    {
      entity.Velocity.Value = Vector2.Zero;
    }
  }
}

