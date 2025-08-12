

using Birds.src.bounding_areas;
using Birds.src.BVH;
using Birds.src.events;
using Birds.src.modules.shared.bounding_area;
using Microsoft.Xna.Framework;
using System.Linq;

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
  public override object Clone()
  {
    CollisionHandlerModule cloned = (CollisionHandlerModule)base.Clone();
    cloned.CollisionManager = new AABBTree();
    cloned.CollisionManager.ResolveInternalCollisions = this.ResolveInternalCollisions;
    return cloned;
  }
}
