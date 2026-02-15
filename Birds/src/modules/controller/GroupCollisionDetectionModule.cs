using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.collision.BVH;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Birds.src.modules.collision;

public class GroupCollisionDetectionModule : BaseCollisionDetectionModule
{
  public AABBTree CollisionManager { get; private set; }

  public override IBoundingArea BoundingArea => BoundingCircle;

  private bool evaluateInternalCollisions;

  public GroupCollisionDetectionModule(bool evaluateInternalCollisions = true)
  {
    CollisionManager = new AABBTree();
    this.evaluateInternalCollisions = evaluateInternalCollisions;
  }

  protected override void Update(GameTime gameTime) => UpdateTreeWithEntities();

  private void UpdateTreeWithEntities()
  {
    var entityCollisionHandlers = container.Entities
        .Select(e => e.GetModule<BaseCollisionDetectionModule>())
        .Where(CDModule => CDModule != null && CDModule.IsCollidable)
        .Cast<ICollidable>()
        .ToList();

    if (entityCollisionHandlers.Count > 0)
    {
      CollisionManager.BuildTree(entityCollisionHandlers);
    }
  }

  public override void AddCollisionsToEntities(ICollidable otherCollidable)
  {
    if (otherCollidable is GroupCollisionDetectionModule otherGroupHandler)
    {
      CollisionManager.AddCollisionsToEntities(otherGroupHandler.CollisionManager);
    }
    else if (otherCollidable is CollisionDetectionModule otherHandler)
    {
      CollisionManager.AddCollisionsToEntities(otherHandler);
    }
    else
      throw new NotImplementedException("EntityCollisionHandlerModule: Collision with non-EntityCollisionHandlerModule not implemented");
  }


  public override void AddInternalCollisions()
  {
    if (evaluateInternalCollisions)
    {
      CollisionManager.AddInternalCollisionsToEntities();
    }
  }

  public override object Clone()
  {
    var cloned = (GroupCollisionDetectionModule)base.Clone();
    cloned.CollisionManager = new AABBTree();
    return cloned;
  }

  public override bool Contains(Vector2 position)
      => container.Entities.Any(e => e.Contains(position));
}