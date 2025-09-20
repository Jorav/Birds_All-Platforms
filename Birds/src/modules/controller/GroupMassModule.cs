using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller;

public class GroupMassModule : ModuleBase
{
  public float Mass { get; set; }

  protected override void ConfigurePropertySync()
  {
    WriteSync(() => Mass, container.Mass);
  }
  protected override void Update(GameTime gameTime)
  {
    float sum = 0;
    foreach (IEntity entity in container.Entities)
    {
      sum += entity.Mass;
    }
    Mass = sum;
  }
}

