using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class SubEntityCollisionExtractionModule : ModuleBase
{
  protected override void Update(GameTime gameTime)
  {
    foreach(IEntity entity in container.Entities)
    {
      foreach (IModuleContainer collisionEntity in entity.Collisions)
      {
        container.Collisions.Add(collisionEntity);
        collisionEntity.Collisions.Add(this.container);
      }
    }
  }
}

