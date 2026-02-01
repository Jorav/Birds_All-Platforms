using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;

public class RadiusModule : ModuleBase
{
  public float Radius { get; set; }

  protected override void ConfigurePropertySync()
  {
    ReadWriteSync(() => Radius, container.Radius);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    UpdateRadius();
  }


  protected override void Update(GameTime gameTime)
  {
    UpdateRadius();
  }

  private void UpdateRadius()
  {
    Radius = (float)Math.Sqrt(Math.Pow(container.Width / 2, 2) + Math.Pow(container.Height / 2, 2));
  }
}

