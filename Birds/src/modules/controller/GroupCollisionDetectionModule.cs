using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.collision.BVH;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.shared.collision_detection;

public class GroupCollisionDetectionModule : BaseCollisionDetectionModule, IEntityCollectionListener
{
  private ICollisionStructure collisionStructure;
  private readonly bool evaluateInternalCollisions;
  private List<ICollidable> childCollidables = new List<ICollidable>(32);

  public override IBoundingArea BoundingArea => BoundingCircle;

  public ICollisionStructure CollisionStructure => collisionStructure;

  public GroupCollisionDetectionModule(
      ICollisionStructure collisionStructure,
      bool evaluateInternalCollisions = true)
  {
    this.collisionStructure = collisionStructure;
    this.evaluateInternalCollisions = evaluateInternalCollisions;
  }

  protected override void Update(GameTime gameTime)
  {
    RefreshChildCollidables();
    collisionStructure.Build(childCollidables);
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
    collisionStructure.AddCollisionsToEntities(otherCollidable);
  }

  public override void AddInternalCollisions()
  {
    if (evaluateInternalCollisions)
    {
      collisionStructure.AddInternalCollisions();
    }
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    RefreshChildCollidables();
  }

  private void RefreshChildCollidables()
  {
    childCollidables.Clear();

    foreach (var entity in container.Entities)
    {
      var cdModule = entity.GetModule<BaseCollisionDetectionModule>();
      if (cdModule != null && cdModule.IsCollidable)
      {
        childCollidables.Add(cdModule);
      }
    }
  }

  public override object Clone()
  {
    var cloned = (GroupCollisionDetectionModule)base.Clone();
    cloned.childCollidables = new List<ICollidable>(32);
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
      childCollidables.Add(cdModule);
    }
  }

  public void OnEntityRemoved(IEntity entity)
  {
    var cdModule = entity.GetModule<BaseCollisionDetectionModule>();
    if (cdModule != null)
    {
      childCollidables.Remove(cdModule);
    }
  }

  public override void Dispose()
  {
    base.Dispose();
    childCollidables.Clear();
    collisionStructure.Dispose();
  }
}