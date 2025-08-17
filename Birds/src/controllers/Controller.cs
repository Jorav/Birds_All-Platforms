using System;
using System.Collections.Generic;
using Birds.src.entities;
using Birds.src.events;
using Birds.src.modules.controller;
using Birds.src.utility;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.controllers;
public class Controller : ModuleContainer, IController
{
  #region Attributes
  protected List<IEntity> entities;
  #endregion
  #region Constructors
  public Controller(List<IEntity> controllables, ID_OTHER team = ID_OTHER.TEAM_AI) : this()
  {
    SetEntities(controllables);
  }
  public Controller() : base()
  {
    entities = new List<IEntity>();
  }

  #endregion
  #region Methods

  public virtual void SetEntities(List<IEntity> newEntities)
  {
    DeprecateEntities();
    foreach (IEntity e in newEntities)
    {
      AddEntity(e);
    }
  }

  public virtual void AddEntity(IEntity e)
  {
    e.Manager = this;
    base.Entities.Add(e);
  }

  private void DeprecateEntities()
  {
    foreach (IEntity entity in Entities)
    {
      entity.Deprecate();
    }
    Entities.Clear();
  }
  public override object Clone()
  {
    var cloned = (Controller)base.Clone();
    cloned.Entities.Clear();
    List<IEntity> clonedEntities = new List<IEntity>();
    foreach (IEntity entity in Entities)
    {
      clonedEntities.Add((IEntity)entity.Clone());
    }
    cloned.SetEntities(clonedEntities);
    return cloned;
  }
  #endregion
}

