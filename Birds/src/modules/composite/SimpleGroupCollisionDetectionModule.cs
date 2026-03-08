using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.composite;

public class SimpleGroupCollisionDetectionModule : BaseCollisionDetectionModule, IEntityCollectionListener
{
  public List<BaseCollisionDetectionModule> entityCollisionHandlers = new List<BaseCollisionDetectionModule>(32);

  public override IBoundingArea BoundingArea => BoundingCircle;

  private bool evaluateInternalCollisions;

  public SimpleGroupCollisionDetectionModule(bool evaluateInternalCollisions = true)
  {
    this.evaluateInternalCollisions = evaluateInternalCollisions;
  }

  protected override void Update(GameTime gameTime)
  {
    LoadCollisionHandlers();
  }

  public override void AddCollisionsToEntities(ICollidable otherCollidable)
  {
    if(otherCollidable is BaseCollisionDetectionModule baseHandler)
    {
      if (!BoundingCircle.CollidesWith(baseHandler.BoundingCircle))
      {
        return;
      }
    }
    if (otherCollidable is SimpleGroupCollisionDetectionModule otherGroupHandler)
    {
      foreach (var myModule in entityCollisionHandlers)
      {
        otherGroupHandler.AddCollisionsToEntities(myModule);
      }
    }
    else if (otherCollidable is CollisionDetectionModule otherHandler)
    {
      foreach (var myModule in entityCollisionHandlers)
      {
        otherHandler.AddCollisionsToEntities(myModule);
      }
    }
    else
    {
      throw new NotImplementedException("SimpleGroupCollisionDetectionModule: Collision with unknown ICollidable type not implemented");
    }
  }

  public override void AddInternalCollisions()
  {
    if (!evaluateInternalCollisions)
    {
      return;
    }
    foreach (var modA in entityCollisionHandlers)
    {
      foreach (var modB in entityCollisionHandlers)
      {
        if (modA == modB) continue;
        modA.AddCollisionsToEntities(modB);
      }
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

  public override object Clone()
  {
    var cloned = (SimpleGroupCollisionDetectionModule)base.Clone();
    cloned.entityCollisionHandlers = new List<BaseCollisionDetectionModule>(32);
    cloned.evaluateInternalCollisions = this.evaluateInternalCollisions;
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
  }
}