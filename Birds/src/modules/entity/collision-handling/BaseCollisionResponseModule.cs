using Birds.src.collision;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.modules.entity.collision_handling;

public abstract class CollisionResponse
{
  public abstract void HandleCollision(IModuleContainer self, IModuleContainer other);
}

