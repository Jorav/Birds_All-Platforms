using Birds.src.modules;
using Birds.src.BVH;
using Birds.src.entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Birds.src.modules.events;
using Birds.src.modules.shared.bounding_area;
using Birds.src.bounding_areas;

namespace Birds.src.modules.collision;
public class CollisionHandlerModule : ControllerModule, ICollidable
{
  public AABBTree CollisionManager { get; private set; }
  public bool ResolveInternalCollisions { get; set; } = true;

  public Vector2 Position => container.Position.Value;
  public float Radius => container.Radius.Value;
  public float Mass => container.Mass.Value;
  public bool IsCollidable { get; set; } = true;
  public IBoundingArea BoundingArea => container.GetModule<BCCollisionDetectionModule>()?.BoundingCircle;

  public CollisionHandlerModule()
  {
    CollisionManager = new AABBTree();
  }

  protected override void ConfigurePropertySync()
  {
    // No property sync needed - reads directly from container
  }

  protected override void Update(GameTime gameTime)
  {
    CollisionManager.ResolveInternalCollisions = ResolveInternalCollisions;
    UpdateTreeWithEntities();
    CollisionManager.Update(gameTime);
  }

  private void UpdateTreeWithEntities()
  {
    var collidableEntities = container.Entities
        .Where(e => e.IsCollidable)
        .Cast<ICollidable>()
        .ToList();

    if (collidableEntities.Count > 0)
    {
      CollisionManager.UpdateTree(collidableEntities);
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
    if (otherEntity is CollisionHandlerModule otherHandler)
    {
      CollisionManager.CollideWithTree(otherHandler.CollisionManager);
    }
  }
}
