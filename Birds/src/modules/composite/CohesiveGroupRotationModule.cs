using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class CoherentGroupRotationModule : RotationModuleBase
{
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
  private float previousRotation;


  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
    ReadWriteSync(() => Rotation, container.Rotation);
  }

  protected override void Update(GameTime gameTime)
  {
    UpdateSubentities();
  }

  public override void RotateTo(Vector2 position)
  {
    Rotation = CalculateRotation(position, Position);
  }

  public void UpdateSubentities()
  {
    float dRotation = previousRotation - Rotation;

    foreach (var e in container.Entities)
    {
      e.Velocity.Value = Vector2.Zero;
      Vector2 relativePosition = e.Position - Position;
      Vector2 newRelativePosition = Vector2.Transform(relativePosition, Matrix.CreateRotationZ(-dRotation));
      e.Velocity.Value += newRelativePosition-relativePosition;
      e.Rotation.Value = Rotation;
    }
    previousRotation = Rotation;
  }
}

