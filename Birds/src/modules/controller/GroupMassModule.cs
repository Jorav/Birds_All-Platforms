using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller;

public class GroupMassModule : ModuleBase
{
  public float Mass { get; set; }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    CalculateMass();
  }

  protected override void ConfigurePropertySync()
  {
    WriteSync(() => Mass, container.Mass);
  }
  protected override void Update(GameTime gameTime)
  {
    CalculateMass();
  }

  private void CalculateMass()
  {
    float sum = 0;
    foreach (IEntity entity in container.Entities)
    {
      sum += entity.Mass;
    }
    Mass = sum;
  }
}

