using Birds.src.bounding_areas;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.shared.bounding_area;
public class OBBCollisionDetectionModule : ModuleBase
{
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
  public OrientedBoundingBox OBB { get; private set; }
  public float Width { get; set; }
  public float Height { get; set; }

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
    if (OBB != null)
    {
      OBB.Position = Position;
      OBB.Rotation = Rotation;
      if (OBB.Width != 0 || OBB.Height != 0)
        return;
      OBB.Width = Width;
      OBB.Height = Height;
    }
  }

  public override object Clone()
  {
    OBBCollisionDetectionModule cloned = (OBBCollisionDetectionModule)base.Clone();
    cloned.OBB = null;
    return cloned;
  }

}

