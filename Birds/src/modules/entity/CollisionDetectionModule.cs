using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.modules.shared.bounding_area;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;

  public class CollisionDetectionModule : BaseCollisionDetectionModule
{
  public BoundingCircle BoundingCircle => container.GetModule<BCCollisionDetectionModule>()?.BoundingCircle;
  public override IBoundingArea BoundingArea => GetSpecificBoundingArea();

  private IBoundingArea GetSpecificBoundingArea()
  {
    var obb = container.GetModule<OBBCollisionDetectionModule>()?.OBB;
    if (obb != null) return obb;

    var circle = container.GetModule<BCCollisionDetectionModule>()?.BoundingCircle;
    return circle;
  }

  protected override void Update(GameTime gameTime)
  {
  }

  public override bool CollidesWith(ICollidable otherCollidable)
  {
    if (!IsCollidable || !otherCollidable.IsCollidable)
    {
      return false;
    }

    if (otherCollidable is CollisionDetectionModule otherHandler
      && otherHandler.BoundingCircle != null
      && BoundingCircle != null)
    {
      return BoundingCircle.CollidesWith(otherHandler.BoundingCircle)
          && BoundingArea.CollidesWith(otherHandler.BoundingArea);
    }
    else
    {
      return BoundingArea?.CollidesWith(otherCollidable.BoundingArea) ?? false;
    }
  }

  public override void AddCollisionsToEntities(ICollidable otherCollidable)
  {
    if (otherCollidable is not CollisionDetectionModule otherHandler)
    {
      throw new NotImplementedException("EntityCollisionHandlerModule: Collision with non-EntityCollisionHandlerModule not implemented");
    }
    if (CollidesWith(otherCollidable))
    {
      container.Collisions.Add(otherHandler.container);
      otherHandler.container.Collisions.Add(container);
    }
  }

  public override object Clone()
  {
    CollisionDetectionModule cloned = (CollisionDetectionModule)base.Clone();
    cloned.IsCollidable = this.IsCollidable;
    return cloned;
  }

  public override void AddInternalCollisions()
  {
  }
}