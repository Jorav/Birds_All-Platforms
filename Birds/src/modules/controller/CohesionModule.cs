using Birds.src.entities;
using Microsoft.Xna.Framework;
using Birds.src.events;
using System;

namespace Birds.src.modules.controller;
public class CohesionModule : ModuleBase
{
  public Vector2 Position { get; set; }
  public static float REPULSIONDISTANCE = 100;
  private float averageDistance;

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    if (container.Entities.Count > 0)
    {
      averageDistance = AverageDistance();
      ApplyInternalGravity();
      ApplyInterParticleGravity();
      ApplyInterParticleRepulsion();
    }
  }

  protected void ApplyInternalGravity()
  {
    Vector2 distanceFromController;
    foreach (IEntity entity in container.Entities)
    {
      distanceFromController = Position - entity.Position;
      if (distanceFromController.Length() > entity.Radius)
      {
        entity.Accelerate(Vector2.Normalize(Position - entity.Position), 0.2f * (float)((distanceFromController.Length() - entity.Radius) / averageDistance) / entity.Mass);
      }
    }
  }

  private void ApplyInterParticleGravity()
  {
    foreach (IEntity entity1 in container.Entities)
      foreach (IEntity entity2 in container.Entities)
        if (entity1 != entity2)
          entity1.AccelerateTo(entity2.Position, 0.005f * averageDistance * entity1.Mass * entity2.Mass / (float)Math.Pow(((entity1.Position.Value - entity2.Position.Value).Length()), 1));
  }

  public void ApplyInterParticleRepulsion()
  {
    foreach (IEntity entity1 in container.Entities)
    {
      foreach (IEntity entity2 in container.Entities)
      {
        if (entity1 == entity2 || entity1.Radius + entity2.Radius + REPULSIONDISTANCE <= Vector2.Distance(entity1.Position, entity2.Position))
        {
          continue;
        }
        Vector2 vectorToE = entity2.Position.Value - entity1.Position.Value;
        float distance = vectorToE.Length();
        float res = 0;
        if (distance < 32)
        {
          distance = 32;
        }
        res = 9f / distance;
        vectorToE.Normalize();
        entity2.Accelerate(vectorToE, entity1.Mass.Value * res);
      }
    }
  }

  protected float AverageDistance()
  {
    float nr = 1;
    float distance = 0;
    float mass = 0;
    foreach (IEntity entity in container.Entities)
    {
      distance += (Vector2.Distance(entity.Position, Position) + entity.Radius) * entity.Mass;
      mass += entity.Mass;
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

