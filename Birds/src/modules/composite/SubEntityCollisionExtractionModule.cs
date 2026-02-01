using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class SubEntityCollisionExtractionModule : ModuleBase
{
  protected override void Update(GameTime gameTime)
  {
    foreach (IEntity entity in container.Entities)
    {
      int collisionCount = entity.Collisions.Count;
      for (int i = 0; i < collisionCount; i++)
      {
        var collisionEntity = entity.Collisions[i];
        container.Collisions.Add(collisionEntity);
        collisionEntity.Collisions.Add(container);
      }
    }
  }
}

