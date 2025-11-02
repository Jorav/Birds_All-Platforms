using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;

public class CohesiveGroupRotationModule : RotationModuleBase
{
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
  private float previousRotation;


  protected override void ConfigurePropertySync()
  {
  ReadSync(() => Position, container.Position);
  ReadWriteSync(() => Rotation, container.Rotation);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    previousRotation = Rotation;
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
    float dRotation = Rotation - previousRotation;
    float deltaTime = (float)Game1.timeStep * 60f;

    foreach (var e in container.Entities)
    {
      Vector2 relativePosition = e.Position.Value - Position;
      Vector2 newRelativePosition = Vector2.Transform(relativePosition, Matrix.CreateRotationZ(dRotation));

      Vector2 velocityChange = (newRelativePosition - relativePosition) / deltaTime;
      e.Velocity.Value += velocityChange;
      e.Rotation.Value = Rotation;
    }
    previousRotation = Rotation;
  }

}