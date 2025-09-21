using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.modules.composite;

public class SubEntityCollisionExtractionModule : ModuleBase
{
  protected override void Update(GameTime gameTime)
  {
    foreach(IEntity entity in container.Entities)
    {
      foreach (IModuleContainer collision in entity.Collisions)
        container.Collisions.Add(collision);
    }
  }
}

