using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Birds.src.containers.composite;

public class CompositeController : ModuleContainer, IEntity
{
  public Controller Manager { get; set; }

  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
  }

  public void Dispose()
  {
    throw new NotImplementedException();
  }

  public override object Clone()
  {
    var cloned = (CompositeController)base.Clone();
    cloned.Manager = this.Manager;
    BlueprintFactory.RestoreConnections(this.Entities.ToList(), cloned.Entities.ToList());

    return cloned;
  }

}
