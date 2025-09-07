using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.collision.BVH;
using Birds.src.events;
using Birds.src.modules.entity;
using Birds.src.modules.shared.bounding_area;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.collision;

public class GroupCollisionDetectionModule : ModuleBase, ICollidable
{
  public AABBTree CollisionManager { get; private set; }
  public Vector2 Position { get; set; }
  public float Radius { get; set; }
  public float Mass { get; set; }
  public bool IsCollidable { get; set; } = true;
  public Stack<(ICollidable, ICollidable)> CollisionPairs {get; set;}
  public IBoundingArea BoundingArea => container.GetModule<BCCollisionDetectionModule>()?.BoundingCircle;

  public GroupCollisionDetectionModule()
  {
    CollisionManager = new AABBTree();
    CollisionPairs = CollisionManager.CollissionPairs;
  }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
    ReadSync(() => Radius, container.Radius);
    ReadSync(() => Mass, container.Mass);
  }

  protected override void Update(GameTime gameTime)
  {
    UpdateTreeWithEntities();
    CollisionManager.GetInternalCollissions();
  }

  private void UpdateTreeWithEntities()
  {
    var entityCollisionHandlers = container.Entities
        .Select(e => e.GetModule<CollisionHandlerModule>())
        .Where(handler => handler != null && handler.IsCollidable)
        .Cast<ICollidable>()
        .ToList();

    if (entityCollisionHandlers.Count > 0)
    {
      CollisionManager.BuildTree(entityCollisionHandlers);
    }
  }

  public bool CollidesWith(ICollidable otherCollidable)
  {
    if (!IsCollidable || !otherCollidable.IsCollidable)
      return false;

    return BoundingArea?.CollidesWith(otherCollidable.BoundingArea) ?? false;
  }

  public void Collide(ICollidable otherEntity)
  {
    if (otherEntity is GroupCollisionDetectionModule otherHandler)
    {
      CollisionManager.GetCollisions(otherHandler.CollisionManager);
    }
  }

  public override object Clone()
  {
    GroupCollisionDetectionModule cloned = (GroupCollisionDetectionModule)base.Clone();
    cloned.CollisionManager = new AABBTree();
    cloned.CollisionPairs = cloned.CollisionManager.CollissionPairs;
    return cloned;
  }
}
