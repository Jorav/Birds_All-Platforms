using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.collision.BVH;
using Birds.src.modules.entity;
using Birds.src.modules.shared.bounding_area;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Birds.src.modules.collision;

public class GroupCollisionDetectionModule : BaseCollisionDetectionModule
{
  public AABBTree CollisionManager { get; private set; }
  public override IBoundingArea BoundingArea => container.GetModule<BCCollisionDetectionModule>()?.BoundingCircle;

  public GroupCollisionDetectionModule()
  {
    CollisionManager = new AABBTree();
  }

  protected override void Update(GameTime gameTime)
  {
    UpdateTreeWithEntities();
  }

  private void UpdateTreeWithEntities()
  {
    var entityCollisionHandlers = container.Entities
        .Select(e => e.GetModule<BaseCollisionDetectionModule>())
        .Where(handler => handler != null && handler.IsCollidable)
        .Cast<ICollidable>()
        .ToList();

    if (entityCollisionHandlers.Count > 0)
    {
      CollisionManager.BuildTree(entityCollisionHandlers);
    }
  }

  public override bool CollidesWith(ICollidable otherCollidable)
  {
    if (!IsCollidable || !otherCollidable.IsCollidable)
      return false;

    return IBoundingArea.CollidesWith(BoundingArea, otherCollidable.BoundingArea);
  }

  public override void AddCollisionsToEntities(ICollidable otherCollidable)
  {
    if (otherCollidable is GroupCollisionDetectionModule otherGroupHandler)
    {
      CollisionManager.AddCollisionsToEntities(otherGroupHandler.CollisionManager);
    }
    else if(otherCollidable is CollisionDetectionModule otherHandler)
    {
      CollisionManager.AddCollisionsToEntities(otherHandler);
    }
    else
      throw new NotImplementedException("EntityCollisionHandlerModule: Collision with non-EntityCollisionHandlerModule not implemented");
  }

  public override object Clone()
  {
    GroupCollisionDetectionModule cloned = (GroupCollisionDetectionModule)base.Clone();
    cloned.CollisionManager = new AABBTree();
    return cloned;
  }

  public override void AddInternalCollisions()
  {
    CollisionManager.AddInternalCollisionsToEntities();
  }
}
