using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.shared.collision_detection;

public abstract class BaseCollisionDetectionModule : ModuleBase, ICollidable
{
  public bool IsCollidable { get; set; } = true;
  public abstract IBoundingArea BoundingArea { get; }

  public BoundingCircle BoundingCircle { get; private set; }

  private Vector2 _position;
  public Vector2 Position
  {
    get => _position;
    set
    {
      _position = value;
      if (BoundingCircle != null)
        BoundingCircle.Position = value;
      if (BoundingArea != null)
        BoundingArea.Position = value;
    }
  }

  private float _radius;
  public float Radius
  {
    get => _radius;
    set
    {
      _radius = value;
      if (BoundingCircle != null)
        BoundingCircle.Radius = value;
    }
  }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
    ReadSync(() => Radius, container.Radius);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    BoundingCircle = BoundingAreaFactory.GetCircle(Position, Radius);
  }

  public virtual bool CollidesWith(ICollidable otherCollidable)
  {
    if (!CullingCircleCollidesWith(otherCollidable))
    {
      return false;
    }
    if (BoundingArea == null)
    {
      return true;
    }
    return IBoundingArea.CollidesWith(BoundingArea, otherCollidable.BoundingArea);
  }

  private bool CullingCircleCollidesWith(ICollidable otherCollidable)
  {
    if (!IsCollidable || !otherCollidable.IsCollidable) return false;

    if (otherCollidable is BaseCollisionDetectionModule other
        && BoundingCircle != null
        && other.BoundingCircle != null)
    {
      return BoundingCircle.CollidesWith(other.BoundingCircle);
    }

    return true;
  }

  public override object Clone()
  {
    var cloned = (BaseCollisionDetectionModule)base.Clone();
    cloned.BoundingCircle = BoundingAreaFactory.GetCircle(Position, Radius);
    return cloned;
  }

  public abstract void AddCollisionsToEntities(ICollidable otherCollidable);
  public abstract void AddInternalCollisions();
  public abstract bool Contains(Vector2 position);
  public override void Dispose()
  {
    if (BoundingCircle != null)
    {
      BoundingCircle.Dispose();
      BoundingCircle = null;
    }
    if (BoundingArea != null)
    {
      BoundingArea.Dispose();
    }
    base.Dispose();
  }
}