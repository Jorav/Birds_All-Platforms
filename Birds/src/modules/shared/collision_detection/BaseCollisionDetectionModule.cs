using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.shared.collision_detection;

public abstract class BaseCollisionDetectionModule : ModuleBase, ICollidable
{
  public Vector2 Position { get; set; }
  public bool IsCollidable { get; set; } = true;
  public abstract IBoundingArea BoundingArea { get; }

  public abstract void AddCollisionsToEntities(ICollidable otherCollidable);

  public abstract void AddInternalCollisions();

  public abstract bool CollidesWith(ICollidable otherCollidable);
}

