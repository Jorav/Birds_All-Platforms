using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity.collision_handling;
using Birds.src.modules.shared.bounding_area;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Birds.src.modules.entity;

  public class CollisionDetectionModule : ModuleBase, ICollidable
{
  public Vector2 Position { get; set; }
  public bool IsCollidable { get; set; } = true;

  public BoundingCircle BoundingCircle => container.GetModule<BCCollisionDetectionModule>()?.BoundingCircle;
  public IBoundingArea BoundingArea => GetSpecificBoundingArea();

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

  public bool CollidesWith(ICollidable otherCollidable)
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

  public void AddCollisionsToEntities(ICollidable otherCollidable)
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

  public void AddInternalCollisions()
  {
  }
}