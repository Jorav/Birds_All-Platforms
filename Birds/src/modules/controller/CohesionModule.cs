using Birds.src.entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Birds.src.modules.events;

namespace Birds.src.modules.controller;
public class CohesionModule : ControllerModule
{
  public Vector2 Position { get; set; }
  public static float REPULSIONDISTANCE = 100;

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    if (container.Entities.Count > 0)
    {
      ApplyInternalGravity();
      ApplyInterParticleGravity();
      ApplyInterParticleRepulsion();
    }
  }

  protected void ApplyInternalGravity()
  {
    Vector2 distanceFromController;
    foreach (IEntity c1 in container.Entities)
    {
      distanceFromController = Position - c1.Position;
      if (distanceFromController.Length() > c1.Radius)
        c1.Accelerate(Vector2.Normalize(Position - c1.Position), 0.5f * (float)((distanceFromController.Length() - c1.Radius) / AverageDistance()) / c1.Mass);
    }
  }

  private void ApplyInterParticleGravity()
  {
    // Implementation here
  }

  public void ApplyInterParticleRepulsion()
  {
    foreach (IEntity e1 in container.Entities)
    {
      foreach (IEntity e2 in container.Entities)
      {
        if (e1 == e2 || e1.Radius + e2.Radius + REPULSIONDISTANCE <= Vector2.Distance(e1.Position, e2.Position))
        {
          continue;
        }
        Vector2 vectorToE = e2.Position - e1.Position;
        float distance = vectorToE.Length();
        float res = 0;
        if (distance < 32)
        {
          distance = 32;
        }
        res = 9f / distance;
        vectorToE.Normalize();
        e2.Accelerate(vectorToE, e1.Mass * res);
      }
    }
  }

  protected float AverageDistance()
  {
    float nr = 1;
    float distance = 0;
    float mass = 0;
    foreach (IEntity c in container.Entities)
    {
      distance += (Vector2.Distance(c.Position, Position) + c.Radius) * c.Mass;
      mass += c.Mass;
    }
    if (mass != 0)
    {
      return distance / nr / mass;
    }
    return 1;
  }

  public virtual object Clone()
  {
    CohesionModule cNew = (CohesionModule)this.MemberwiseClone();
    return cNew;
  }
}

