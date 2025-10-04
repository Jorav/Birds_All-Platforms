using Birds.src.events;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Birds.src.modules.controller;

public class GroupCollisionClearer : ModuleBase
{
  protected override void Update(GameTime gameTime)
  {
    foreach(var entity in container.Entities)
    {
      entity.Collisions.Clear();
    }
    container.Collisions.Clear();
  }
}

