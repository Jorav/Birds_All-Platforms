using Birds.src.collision.bounding_areas;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.collision;
public interface ICollidable
{
  Vector2 Position { get; }
  float Radius { get; }
  float Mass { get; }
  IBoundingArea BoundingArea { get; }
  bool IsCollidable { get; }

  public bool CollidesWith(ICollidable otherCollidable);
  public void Collide(ICollidable otherEntity);

  public static void ResolveCollisions(Stack<(ICollidable, ICollidable)> collisionPairs)
  {
    while (collisionPairs.Count > 0)
    {
      (ICollidable, ICollidable) pair = collisionPairs.Pop();
      pair.Item1.Collide(pair.Item2);
      pair.Item2.Collide(pair.Item1);
    }
  }
}