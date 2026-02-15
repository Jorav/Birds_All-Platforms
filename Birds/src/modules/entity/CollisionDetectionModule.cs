using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.modules.collision;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;

public class CollisionDetectionModule : BaseCollisionDetectionModule
{
  private IBoundingArea _preciseBoundingArea;

  public override IBoundingArea BoundingArea => _preciseBoundingArea ?? BoundingCircle;

  public new Vector2 Position
  {
    get => base.Position;
    set
    {
      base.Position = value;
      if (_preciseBoundingArea != null)
        _preciseBoundingArea.Position = value;
    }
  }

  public float Rotation
  {
    get => _rotation;
    set
    {
      _rotation = value;
      if (_preciseBoundingArea is IRectangle rect)
        rect.Rotation = value;
    }
  }
  private float _rotation;

  public CollisionDetectionModule(IBoundingArea preciseBoundingArea = null)
  {
    _preciseBoundingArea = preciseBoundingArea;
  }

  protected override void ConfigurePropertySync()
  {
    base.ConfigurePropertySync();
    ReadSync(() => Rotation, container.Rotation);
  }

  protected override void Update(GameTime gameTime) { }

  public override void AddCollisionsToEntities(ICollidable otherCollidable)
  {
    if (otherCollidable is CollisionDetectionModule otherHandler)
    {
      if (CollidesWith(otherHandler))
      {
        container.Collisions.Add(otherHandler.container);
        otherHandler.container.Collisions.Add(container);
      }
    }
    else if (otherCollidable is GroupCollisionDetectionModule otherGroupHandler)
    {
      otherGroupHandler.AddCollisionsToEntities(this);
    }
    else
    {
      throw new NotImplementedException(
          "CollisionDetectionModule: Collision with unknown ICollidable type not implemented");
    }
  }

  public override void AddInternalCollisions() { }

  public override object Clone()
  {
    var clone = (CollisionDetectionModule)base.Clone();
    clone._preciseBoundingArea = (IBoundingArea)_preciseBoundingArea.Clone();
    return clone;
  }

  public override bool Contains(Vector2 position) => BoundingArea.Contains(position);
}