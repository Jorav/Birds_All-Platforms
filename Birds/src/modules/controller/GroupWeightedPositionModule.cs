using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.modules.controller;

public class GroupWeightedPositionModule : ModuleBase, IEntityCollectionListener
{
  public Vector2 Position { get; set; }
  public float Mass { get; set; }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    UpdatePosition();
  }

  protected override void ConfigurePropertySync()
  {
    ReadWriteSync(() => Position, container.Position);
    ReadSync(() => Mass, container.Mass);
  }

  protected override void Update(GameTime gameTime)
  {
    UpdatePosition();
  }

  public void OnEntityAdded(IEntity newEntity)
  {
    if (container.Entities.Count == 1)
    {
      Position = newEntity.Position.Value;
      return;
    }
    Position = (container.Position.Value * (container.Mass - newEntity.Mass)
                + newEntity.Position.Value * newEntity.Mass)
               / container.Mass;
  }

  public void OnEntityRemoved(IEntity entity)
  {
    if (container.Entities.Count == 0)
    {
      Position = Vector2.Zero;
      return;
    }
    Position = (container.Position.Value * (container.Mass + entity.Mass)
                - entity.Position.Value * entity.Mass)
               / container.Mass;
  }

  private void UpdatePosition()
  {
    Position = CalculateCenterOfMass(container.Entities);
  }

  public static Vector2 CalculateCenterOfMass(IEnumerable<IEntity> entities)
  {
    Vector2 sum = Vector2.Zero;
    float weight = 0;
    foreach (IEntity entity in entities)
    {
      weight += entity.Mass.Value;
      sum += entity.Position.Value * entity.Mass.Value;
    }
    return weight > 0 ? sum / weight : Vector2.Zero;
  }
}