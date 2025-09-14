using Birds.src.collision;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.modules.entity.collision_handling;

public abstract class BaseCollisionResponseModule : ModuleBase
{
  public abstract void HandleCollision(ICollidable self, ICollidable other);

}

