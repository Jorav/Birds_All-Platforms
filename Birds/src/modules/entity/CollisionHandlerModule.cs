using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.events;
using Birds.src.modules.shared.bounding_area;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity
{
  public class CollisionHandlerModule : ModuleBase, ICollidable
  {
    public Vector2 Position { get; set; }
    public float Radius { get; set; }
    public float Mass { get; set; }
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

    protected override void ConfigurePropertySync()
    {
      ReadSync(() => Position, container.Position);
      ReadSync(() => Radius, container.Radius);
      ReadSync(() => Mass, container.Mass);
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

      if (otherCollidable is CollisionHandlerModule otherHandler
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

    public void Collide(ICollidable otherCollidable)
    {
      if (otherCollidable is not CollisionHandlerModule otherHandler)
      {
        throw new NotImplementedException("EntityCollisionHandlerModule: Collision with non-EntityCollisionHandlerModule not implemented");
      }
      var movementModule = container.GetModule<MovementModule>();
      var otherMovementModule = otherHandler.container.GetModule<MovementModule>();

      if (movementModule != null && otherMovementModule != null)
      {
        movementModule.TotalExteriorForce += movementModule.CalculateCollissionRepulsion(otherMovementModule);

        if (BoundingCircle != null && otherHandler.BoundingCircle != null)
        {
          movementModule.TotalExteriorForce += BoundingCircle.CalculateOverlapRepulsion(otherHandler.BoundingCircle);
        }
      }

    }

    public override object Clone()
    {
      CollisionHandlerModule cloned = (CollisionHandlerModule)base.Clone();
      cloned.IsCollidable = this.IsCollidable;
      return cloned;
    }
  }
}
