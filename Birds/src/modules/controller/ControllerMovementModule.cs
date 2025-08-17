using Birds.src.entities;
using Microsoft.Xna.Framework;
using Birds.src.events;

namespace Birds.src.modules.controller;
public class ControllerMovementModule : ControllerModule, IMovementModule
{

  public Vector2 Position { get; set; }
  public float Radius { get; set; }
  public float Mass { get; set; }
  protected override void ConfigurePropertySync()
  {
    WriteSync(() => Position, container.Position);
    WriteSync(() => Radius, container.Radius);
    WriteSync(() => Mass, container.Mass);
    //WriteSync(() => Thrust, container.Thrust);
  }

  public void AccelerateTo(Vector2 position, float thrust)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.AccelerateTo(position, thrust);
    }
  }

  public void Accelerate(Vector2 directionalVector, float thrust)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.Accelerate(directionalVector, thrust);
    }
  }

  public void Accelerate(Vector2 directionalVector)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.Accelerate(directionalVector);
    }
  }

  public void RotateTo(Vector2 position)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.RotateTo(position);
    }
  }

  protected override void Update(GameTime gameTime)
  {
    foreach (IEntity entity in container.Entities)
    {
      entity.Update(gameTime);
    }

    UpdatePosition();
    UpdateRadius();
    UpdateMass();
  }

  private void UpdatePosition()
  {
    Vector2 sum = Vector2.Zero;
    float weight = 0;

    foreach (IEntity entity in container.Entities)
    {
      weight += entity.Mass;
      sum += entity.Position.Value * entity.Mass.Value;
    }

    if (weight > 0)
    {
      Position = sum / weight;  // Mass center
    }
    else
    {
      Position = Vector2.Zero;
    }
  }

  private void UpdateRadius()
  {
    float largestDistance = 0;

    foreach (IEntity entity in container.Entities)
    {
      float distance = Vector2.Distance(entity.Position, Position) + entity.Radius;
      if (distance > largestDistance)
        largestDistance = distance;
    }

    Radius = largestDistance;
  }

  private void UpdateMass()
  {
    float sum = 0;
    foreach (IEntity entity in container.Entities)
    {
      sum += entity.Mass;
    }
    Mass = sum;
  }
}

