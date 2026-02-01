using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

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
    Position = (container.Position.Value * (container.Mass - newEntity.Mass) + newEntity.Position.Value * newEntity.Mass) / (newEntity.Mass + container.Mass);
  }

  public void OnEntityRemoved(IEntity entity)
  {
    Position = (container.Position.Value * container.Mass - entity.Position.Value * entity.Mass) / (entity.Mass + container.Mass);
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
      Position = sum / weight;
    }
  }
}

