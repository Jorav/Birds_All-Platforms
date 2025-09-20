﻿using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;

public class RotationModule : RotationModuleBase
{
  public virtual float Rotation { get; set; }
  protected float rotation;
  public virtual Vector2 Position { get; set; }


  protected override void ConfigurePropertySync()
  {
    ReadWriteSync(() => Rotation, container.Rotation);
    ReadSync(() => Position, container.Position);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    Rotation = 0;
  }

  protected override void Update(GameTime gameTime)
  {
  }

  public override void RotateTo(Vector2 position)
  {
    Rotation = CalculateRotation(position, Position);
  }

  public override object Clone()
  {
    var cloned = new RotationModule();
    cloned.Rotation = this.Rotation;
    return cloned;
  }
}

