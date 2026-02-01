using Birds.src.collision.bounding_areas;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.shared.bounding_area;

public class BCCollisionDetectionModule : ModuleBase
{
  private Vector2 _position;
  public Vector2 Position
  {
    get => _position;
    set
    {
      _position = value;
      if (BoundingCircle != null)
        BoundingCircle.Position = value;
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

  public BoundingCircle BoundingCircle { get; private set; }

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

  protected override void Update(GameTime gameTime)
  {
  }

  public override object Clone()
  {
    var cloned = (BCCollisionDetectionModule)base.Clone();
    cloned.BoundingCircle = BoundingAreaFactory.GetCircle(this.Position, this.Radius);
    return cloned;
  }
}