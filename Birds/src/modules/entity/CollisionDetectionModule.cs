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
          && IBoundingArea.CollidesWith(BoundingArea, otherHandler.BoundingArea);
    }
    else
    {
      return IBoundingArea.CollidesWith(BoundingArea, otherCollidable.BoundingArea);
    }
  }

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
    else if(otherCollidable is GroupCollisionDetectionModule otherGroupHandler)
    {
      otherGroupHandler.AddCollisionsToEntities(this);
    }
    else
    {
      throw new NotImplementedException("EntityCollisionHandlerModule: Collision with non-EntityCollisionHandlerModule not implemented");
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

  public override bool Contains(Vector2 position)
  {
    return BoundingArea.Contains(position);
  }
}