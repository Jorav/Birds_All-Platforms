using Birds.src.collision;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.modules.controller;
public class GroupCollisionHandlerModule
{
  public bool ResolveInternalCollisions { get; set; } = true;
  public Stack<(ICollidable, ICollidable)> CollissionPairs { get; set; }

  public void ResolveCollissions()
  {
    if (!ResolveInternalCollisions)
    {
      return;
    }
    while (CollissionPairs.Count > 0)
    {
      (ICollidable, ICollidable) pair = CollissionPairs.Pop();
      pair.Item1.Collide(pair.Item2);
      pair.Item2.Collide(pair.Item1);
    }
  }
}

