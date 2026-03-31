using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Birds.src.containers.composite;

public class CompositeController : ModuleContainer, IEntity
{
  public ID_ENTITY EntityID { get; set; } //TODO: remove this OR make sure composites can be displayed similar to worldentities in editentitystate
  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
  }

  public override void Dispose()
  {
    base.Dispose();
    CompositeControllerFactory.availableEntities.Push(this);
  }

  public override object Clone()
  {
    var cloned = (CompositeController)base.Clone();
    BlueprintFactory.RestoreConnections(this.Entities.ToList(), cloned.Entities.ToList());

    return cloned;
  }
}
