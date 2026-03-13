using Birds.src.cohesion;
using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.controller;

public class FlockingCohesionModule : ModuleBase
{
  public Vector2 Position { get; set; }
  public static float RADIUS_MULTIPLIER = 8f;
  public static int K_NEIGHBORS = 15;
  public static float REPULSIONDISTANCE = 32f;

  private KNearestTree flockingTree;

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    flockingTree = new KNearestTree();
  }

  protected override void Update(GameTime gameTime)
  {
    if (container.Entities.Count > 0)
    {
      flockingTree.Build(container.Entities);

      foreach (var entity in container.Entities)
      {
        float searchRadius = entity.Radius.Value * RADIUS_MULTIPLIER;
        var neighbors = flockingTree.GetEntitiesWithinRadius(entity, searchRadius);

        foreach (var neighbor in neighbors)
        {
          ApplyForcesBetween(entity, neighbor);
        }
      }
    }
  }

  private void ApplyForcesBetween(IEntity entity1, IEntity entity2)
  {
    Vector2 direction = entity2.Position.Value - entity1.Position.Value;
    float distance = direction.Length();

    if (distance > 2 * (entity1.Radius.Value + entity2.Radius.Value + REPULSIONDISTANCE))
      return;

    if (distance < 0.1f)
    {
      var random = new Random();
      float angle = (float)(random.NextDouble() * Math.PI * 2);
      direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
      distance = 0.1f;
    }

    if (distance < (entity1.Radius.Value + entity2.Radius.Value) / 2)
    {
      distance = (entity1.Radius.Value + entity2.Radius.Value) / 2;
    }

    direction.Normalize();

    float gravityForce = 1f * entity1.Mass.Value * entity2.Mass.Value / distance;
    entity1.Accelerate(direction, gravityForce / entity1.Mass.Value);
    entity2.Accelerate(-direction, gravityForce / entity2.Mass.Value);

    float repulsionForce = 500f * entity1.Mass.Value * entity2.Mass.Value / (distance * distance);
    entity1.Accelerate(-direction, repulsionForce / entity1.Mass.Value);
    entity2.Accelerate(direction, repulsionForce / entity2.Mass.Value);
  }

  public override object Clone()
  {
    var cloned = (FlockingCohesionModule)base.Clone();
    cloned.flockingTree = new KNearestTree();
    return cloned;
  }

  public override void Dispose()
  {
    base.Dispose();
    flockingTree = null;
  }
}