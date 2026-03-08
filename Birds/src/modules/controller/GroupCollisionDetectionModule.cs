using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.collision.BVH;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.composite;
using Birds.src.modules.entity;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.collision;

public class GroupCollisionDetectionModule : BaseCollisionDetectionModule, IEntityCollectionListener
{
  public AABBTree CollisionManager { get; private set; }
  private List<ICollidable> entityCollisionHandlers = new List<ICollidable>(32);

  public override IBoundingArea BoundingArea => BoundingCircle;

  private bool evaluateInternalCollisions;

  public GroupCollisionDetectionModule(bool evaluateInternalCollisions = true)
  {
    CollisionManager = new AABBTree();
    this.evaluateInternalCollisions = evaluateInternalCollisions;
  }

  protected override void Update(GameTime gameTime)
  {
    LoadCollisionHandlers();
    UpdateTreeWithEntities();
  }

  private void UpdateTreeWithEntities()
  {
    if (entityCollisionHandlers.Count > 0)
    {
      CollisionManager.BuildTree(entityCollisionHandlers);
    }
  }

  public override void AddCollisionsToEntities(ICollidable otherCollidable)
  {
    if (otherCollidable is BaseCollisionDetectionModule baseHandler)
    {
      if (!BoundingCircle.CollidesWith(baseHandler.BoundingCircle))
      {
        return;
      }
    }
    if (otherCollidable is GroupCollisionDetectionModule otherGroupCollisionModule)
    {
      CollisionManager.AddCollisionsToEntities(otherGroupCollisionModule.CollisionManager);
    }
    else if (otherCollidable is CollisionDetectionModule otherCollisionModule)
    {
      CollisionManager.AddCollisionsToEntities(otherCollisionModule);
    }
    else if (otherCollidable is SimpleGroupCollisionDetectionModule otherSimpleGroupCollisionModule)
    {
      foreach(ICollidable subEntityHandler in otherSimpleGroupCollisionModule.entityCollisionHandlers)
      {
        CollisionManager.AddCollisionsToEntities(subEntityHandler);

      }
    }
    else
    {
      throw new NotImplementedException("EntityCollisionHandlerModule: Collision with non-EntityCollisionHandlerModule not implemented");
    }
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    LoadCollisionHandlers();
  }

  private void LoadCollisionHandlers()
  {
    entityCollisionHandlers.Clear();

    foreach (var entity in container.Entities)
    {
      var cdModule = entity.GetModule<BaseCollisionDetectionModule>();
      if (cdModule != null && cdModule.IsCollidable)
      {
        entityCollisionHandlers.Add(cdModule);
      }
    }
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
    cloned.entityCollisionHandlers = new List<ICollidable>(32);
    return cloned;
  }

  public override bool Contains(Vector2 position)
  {
    if (!BoundingCircle.Contains(position))
    {
      return false;
    }

    return container.Entities.Any(e => e.Contains(position));
  }

  public void OnEntityAdded(IEntity entity)
  {
    var cdModule = entity.GetModule<BaseCollisionDetectionModule>();
    if (cdModule != null && cdModule.IsCollidable)
    {
      entityCollisionHandlers.Add(cdModule);
    }
  }

  public void OnEntityRemoved(IEntity entity)
  {
    var cdModule = entity.GetModule<BaseCollisionDetectionModule>();

    if (cdModule != null)
    {
      bool removed = entityCollisionHandlers.Remove(cdModule);
    }
  }

  public override void Dispose()
  {
    base.Dispose();
    entityCollisionHandlers.Clear();
    CollisionManager.Dispose();
  }
}