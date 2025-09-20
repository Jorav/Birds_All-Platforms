using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Birds.src.modules.composite;

public class CompositeRotationModule : RotationModuleBase
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
      Vector2 relativePosition = e.Position - container.Position.Value;
      Vector2 newRelativePosition = Vector2.Transform(relativePosition, Matrix.CreateRotationZ(-dRotation));
      e.Rotation.Value = Rotation;
      //e.MovementModule.Velocity = newRelativePosition - relativePosition;
      e.Position.Value = Position + newRelativePosition; //replace with above after
    }
    previousRotation = Rotation;
  }
}

