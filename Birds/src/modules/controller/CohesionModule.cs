using Microsoft.Xna.Framework;
using Birds.src.events;
using System;
using System.Collections.Generic;
using Birds.src.containers.entity;

namespace Birds.src.modules.controller;
public class CohesionModule : ModuleBase
{
  public Vector2 Position { get; set; }
  public static float REPULSIONDISTANCE = 2f;
  public static float OUTLIER_THRESHOLD = 1.5f;
  private float averageDistance;
  private Dictionary<IEntity, float> entityWeightedAverageDistances = new Dictionary<IEntity, float>();

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    if (container.Entities.Count > 0)
    {
      averageDistance = AverageDistance();
      ApplyInterParticleGravity();
      ApplyInterParticleRepulsion();
      ApplyInternalGravity();
    }
  }

  protected void ApplyInternalGravity()
  {
    float globalWeightedAverage = CalculateGlobalWeightedAverageDistance();

    foreach (var kvp in entityWeightedAverageDistances)
    {
      IEntity entity = kvp.Key;
      float weightedAvgDistance = kvp.Value;

      if (weightedAvgDistance > globalWeightedAverage * OUTLIER_THRESHOLD)
      {
        Vector2 distanceFromController = Position - entity.Position.Value;
        float cohesionStrength = 1f * (weightedAvgDistance - globalWeightedAverage) / globalWeightedAverage;
        entity.Accelerate(Vector2.Normalize(distanceFromController), cohesionStrength / entity.Mass.Value);
      }
    }
  }

  private float CalculateGlobalWeightedAverageDistance()
  {
    float totalWeightedDistance = 0;
    float totalWeight = 0;

    foreach (IEntity entity1 in container.Entities)
    {
      foreach (IEntity entity2 in container.Entities)
      {
        if (entity1 != entity2)
        {
          float distance = Vector2.Distance(entity1.Position.Value, entity2.Position.Value);
          float weight = entity2.Mass.Value;
          totalWeightedDistance += distance * weight;
          totalWeight += weight;
        }
      }
    }

    return totalWeight > 0 ? totalWeightedDistance / totalWeight : 100f;
  }

  private void ApplyInterParticleGravity()
  {
    entityWeightedAverageDistances.Clear();

    foreach (IEntity entity1 in container.Entities)
    {
      float weightedDistanceSum = 0;
      float totalWeight = 0;

      foreach (IEntity entity2 in container.Entities)
      {
        if (entity1 != entity2)
        {
          Vector2 direction = entity2.Position.Value - entity1.Position.Value;
          float distance = direction.Length();

          weightedDistanceSum += distance * entity2.Mass.Value;
          totalWeight += entity2.Mass.Value;

          if (distance == 0)
          {
            continue;
          }
          direction.Normalize();
          float force = 1f * entity1.Mass.Value * entity2.Mass.Value / distance;
          entity1.Accelerate(direction, force / entity1.Mass.Value);
          entity2.Accelerate(-direction, force / entity2.Mass.Value);
        }
      }

      entityWeightedAverageDistances[entity1] = totalWeight > 0 ? weightedDistanceSum / totalWeight : 0;
    }
  }

  public void ApplyInterParticleRepulsion()
  {
    foreach (IEntity entity1 in container.Entities)
    {
      foreach (IEntity entity2 in container.Entities)
      {
        if (entity1 == entity2)
        {
          continue;
        }
        float distance = Vector2.Distance(entity1.Position.Value, entity2.Position.Value);
        float minDistance = (entity1.Radius.Value + entity2.Radius.Value) * REPULSIONDISTANCE;

        if (distance >= minDistance)
        {
          continue;
        }
        Vector2 repulsionDirection = entity1.Position.Value - entity2.Position.Value;
        if (repulsionDirection.Length() < 0.1f)
        {
          repulsionDirection = new Vector2(0.1f, 0.05f);
        }
        repulsionDirection.Normalize();

        float overlap = minDistance - distance;
        if (distance < entity1.Radius.Value + entity2.Radius.Value)
        {
          distance = entity1.Radius.Value + entity2.Radius.Value;
        }
        float repulsionForce = 0.7f * overlap / distance;

        entity1.Accelerate(repulsionDirection, repulsionForce / entity1.Mass.Value);
        entity2.Accelerate(-repulsionDirection, repulsionForce / entity2.Mass.Value);
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
