using Birds.src.collision.bounding_areas;
using Microsoft.Xna.Framework;

namespace Birds.src.collision;
public interface ICollidable
{
  Vector2 Position { get; }
  IBoundingArea BoundingArea { get; }
  bool IsCollidable { get; }

  public bool CollidesWith(ICollidable otherCollidable);
  public void AddCollisionsToEntities(ICollidable otherCollidable);
  public void AddInternalCollisions();
}