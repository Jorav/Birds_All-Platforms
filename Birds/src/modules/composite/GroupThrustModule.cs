using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.composite;
public class GroupThrustModule : ModuleBase
{
  public float Thrust { get; set; }

  protected override void ConfigurePropertySync()
  {
    WriteSync(() => Thrust, container.Thrust);
  }

  protected override void Update(GameTime gameTime)
  {
    float sum = 0;
    foreach (IEntity entity in container.Entities)
    {
      sum += entity.Thrust.Value;
    }
    Thrust = sum;
  }
}