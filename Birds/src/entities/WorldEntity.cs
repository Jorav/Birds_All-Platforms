using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.modules.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Birds.src.entities;
public class WorldEntity : ModuleContainer, IEntity
{
  public bool IsFiller { get; set; }
  public ID_ENTITY EntityID { get; set; }
  public Controller Manager { get; set; }

  public WorldEntity()
  {
  }

  public virtual object Clone()
  {
    var cloned = (WorldEntity)base.Clone();
    cloned.IsFiller = this.IsFiller;
    cloned.EntityID = this.EntityID;
    cloned.Manager = null;
    return cloned;
  }
  public void Deprecate()
  {
    EntityFactory.availableEntities.Push(this);
  }
}

