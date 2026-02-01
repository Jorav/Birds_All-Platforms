using Birds.src.collision.bounding_areas;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.shared.bounding_area;

public class OBBCollisionDetectionModule : ModuleBase
{
  private Vector2 _position;
  public Vector2 Position
  {
    get => _position;
    set
    {
      _position = value;
      if (OBB != null)
        OBB.Position = value;
    }
  }

  private float _rotation;
  public float Rotation
  {
    get => _rotation;
    set
    {
      _rotation = value;
      if (OBB != null)
        OBB.Rotation = value;
    }
  }

  private float _width;
  public float Width
  {
    get => _width;
    set
    {
      _width = value;
      if (OBB != null)
        OBB.Width = value;
    }
  }

  private float _height;
  public float Height
  {
    get => _height;
    set
    {
      _height = value;
      if (OBB != null)
        OBB.Height = value;
    }
  }

  public OrientedBoundingBox OBB { get; private set; }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
    ReadSync(() => Rotation, container.Rotation);
    ReadSync(() => Width, container.Width);
    ReadSync(() => Height, container.Height);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    OBB = BoundingAreaFactory.GetOBB(Position, Rotation, (int)Width, (int)Height);
  }

  protected override void Update(GameTime gameTime)
  {
  }

  public override object Clone()
  {
    OBBCollisionDetectionModule cloned = (OBBCollisionDetectionModule)base.Clone();
    cloned.OBB = BoundingAreaFactory.GetOBB(this.Position, this.Rotation, (int)this.Width, (int)this.Height);
    return cloned;
  }
}