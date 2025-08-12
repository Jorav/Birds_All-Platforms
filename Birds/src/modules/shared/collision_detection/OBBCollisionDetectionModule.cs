using Birds.src.bounding_areas;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.shared.bounding_area;
public class OBBCollisionDetectionModule : ControllerModule
{
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
  public OrientedBoundingBox OBB { get; private set; }
  private readonly int _width, _height;

  public OBBCollisionDetectionModule(int width, int height)
  {
    _width = width;
    _height = height;
  }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
    ReadSync(() => Rotation, container.Rotation);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    OBB = BoundingAreaFactory.GetOBB(Position, Rotation, _width, _height);
  }

  protected override void Update(GameTime gameTime)
  {
    if (OBB != null)
    {
      OBB.Position = Position;
      OBB.Rotation = Rotation;
    }
  }

  public override object Clone()
  {
    OBBCollisionDetectionModule cloned = (OBBCollisionDetectionModule)base.Clone();
    cloned.OBB = null;
    return cloned;
  }

}

