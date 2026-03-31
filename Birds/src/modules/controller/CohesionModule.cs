using Microsoft.Xna.Framework;
using Birds.src.events;
using System.Collections.Generic;
using Birds.src.containers.entity;
using System;
using System.Linq;

namespace Birds.src.modules.controller;

public class CohesionModule : ModuleBase
{
  public Vector2 Position { get; set; }
  public static float REPULSIONDISTANCE = 32f;
  public static float OUTLIER_THRESHOLD = 4f;

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    if (container.Entities.Count > 0)
    {
      ApplyInterParticleGravity();
      ApplyInterParticleRepulsion();
      //ApplyOutlierCohesion();
    }
  }

  private void ApplyOutlierCohesion()
  {
    foreach (IEntity entity in container.Entities)
    {
      float totalWeightedRelativeDistance = 0;
      float totalMass = 0;

      foreach (IEntity other in container.Entities)
      {
        if (entity == other) continue;

        float distance = Vector2.Distance(entity.Position.Value, other.Position.Value);
        float combinedRadius = entity.Radius.Value + other.Radius.Value;
        float relativeDistance = distance / combinedRadius;

        totalWeightedRelativeDistance += relativeDistance * other.Mass.Value;
        totalMass += other.Mass.Value;
      }

      if (totalMass == 0) continue;

      float avgRelativeDistance = totalWeightedRelativeDistance / totalMass;

      if (avgRelativeDistance > OUTLIER_THRESHOLD)
      {
        Vector2 toCenter = Position - entity.Position.Value;
        float distanceToCenter = toCenter.Length();

        if (distanceToCenter < 0.1f) continue;

        Vector2 direction = toCenter / distanceToCenter;
        float excessDistance = avgRelativeDistance - OUTLIER_THRESHOLD;
        float cohesionForce = 2f * excessDistance;

        entity.Accelerate(direction, cohesionForce / entity.Mass.Value);
      }
    }
  }

  private void ApplyInterParticleGravity()
  {
    foreach (IEntity entity1 in container.Entities)
    {
      foreach (IEntity entity2 in container.Entities)
      {
        if (entity1 == entity2) continue;

        Vector2 direction = entity2.Position.Value - entity1.Position.Value;
        float distance = direction.Length();
        if (distance > 2 * (entity1.Radius + entity2.Radius + REPULSIONDISTANCE))
          continue;
        if (distance < 0.1f)
        {
          var random = new Random();
          float angle = (float)(random.NextDouble() * Math.PI * 2);
          direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        if (distance < (entity1.Radius.Value + entity2.Radius.Value) / 2)
        {
          distance = (entity1.Radius.Value + entity2.Radius.Value) / 2;
        }

        direction.Normalize();
        float force = 1f * entity1.Mass.Value * entity2.Mass.Value / distance;
        entity1.Accelerate(direction, force / entity1.Mass.Value);
        entity2.Accelerate(-direction, force / entity2.Mass.Value);
      }
    }
  }

  public void ApplyInterParticleRepulsion()
  {
    foreach (IEntity entity1 in container.Entities)
    {
      foreach (IEntity entity2 in container.Entities)
      {
        if (entity1 == entity2) continue;

        float distance = Vector2.Distance(entity1.Position.Value, entity2.Position.Value);
        if (distance > 2 * (entity1.Radius + entity2.Radius + REPULSIONDISTANCE))
          continue;
        Vector2 repulsionDirection = entity1.Position.Value - entity2.Position.Value;
        if (distance < 0.1f)
        {
          var random = new Random();
          float angle = (float)(random.NextDouble() * Math.PI * 2);
          repulsionDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        if (distance < (entity1.Radius.Value + entity2.Radius.Value)/2)
        {
          distance = (entity1.Radius.Value + entity2.Radius.Value)/2;
        }
        float repulsionForce = 16*16f * entity1.Mass.Value * entity2.Mass.Value / distance / distance;

        entity1.Accelerate(repulsionDirection, repulsionForce / entity1.Mass.Value);
        entity2.Accelerate(-repulsionDirection, repulsionForce / entity2.Mass.Value);
      }
    }
  }

  public override object Clone()
  {
    return (CohesionModule)base.Clone();
  }

  public override void Dispose()
  {
    base.Dispose();
  }
}