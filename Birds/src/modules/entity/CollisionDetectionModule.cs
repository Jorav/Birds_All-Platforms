using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.modules.collision;
using Birds.src.modules.shared.bounding_area;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;

public class CollisionDetectionModule : BaseCollisionDetectionModule
{
  public override IBoundingArea BoundingArea =>
      container.GetModule<OBBCollisionDetectionModule>()?.OBB
      ?? (IBoundingArea)BoundingCircle;

  protected override void Update(GameTime gameTime) { }

  public override void AddCollisionsToEntities(ICollidable otherCollidable)
  {
    if (otherCollidable is CollisionDetectionModule otherHandler)
    {
      if (CollidesWith(otherHandler))
      {
        container.Collisions.Add(otherHandler.container);
        otherHandler.container.Collisions.Add(container);
      }
    }
    else if (otherCollidable is GroupCollisionDetectionModule otherGroupHandler)
    {
      otherGroupHandler.AddCollisionsToEntities(this);
    }
    else
    {
      throw new NotImplementedException(
          "CollisionDetectionModule: Collision with unknown ICollidable type not implemented");
    }
  }

  public override void AddInternalCollisions() { }

  public override bool Contains(Vector2 position) => BoundingArea.Contains(position);
}