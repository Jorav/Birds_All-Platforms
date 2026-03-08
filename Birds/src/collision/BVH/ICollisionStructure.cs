using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Birds.src.collision.BVH;

public interface ICollisionStructure : IDisposable
{
  void Build(List<ICollidable> entities);
  void AddCollisionsToEntities(ICollidable other);
  void AddInternalCollisions();
}