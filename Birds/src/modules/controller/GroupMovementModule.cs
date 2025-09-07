using Microsoft.Xna.Framework;
using Birds.src.events;
using Birds.src.containers.entity;

namespace Birds.src.modules.controller;
public class GroupMovementModule : ModuleBase, IMovementModule
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

  protected override void Update(GameTime gameTime)
  {
    UpdatePosition();
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

