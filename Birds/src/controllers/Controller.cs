using System;
using System.Collections.Generic;
using Birds.src.entities;
using Birds.src.events;
using Birds.src.utility;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.controllers;
public class Controller : ModuleContainerBase, IController
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

  public void Draw(SpriteBatch sb)
  {
    foreach (IEntity entity in Entities)
    {
      entity.Draw(sb);
    }
  }

  private void DeprecateEntities()
  {
    foreach (IEntity entity in Entities)
    {
      entity.Deprecate();
    }
    Entities.Clear();
  }

  public virtual object Clone()
  {
    /*
    Controller cNew = (Controller)this.MemberwiseClone();
    cNew.CollisionManager = new AABBTree();
    List<IEntity> entities = new List<IEntity>();
    foreach (IEntity c in Entities)
      entities.Add((IEntity)c.Clone());
    cNew.SetEntities(entities);
    cNew.BoundingCircle = BoundingAreaFactory.GetCircle(cNew.Position, cNew.radius);
    if (Steering != null)
    {
      cNew.Steering = (SteeringModule)Steering.Clone();
      cNew.Steering.controller = cNew;
    }
    if (CohesionModule != null)
    {
      cNew.CohesionModule = (CohesionModule)CohesionModule.Clone();
      cNew.CohesionModule.controller = cNew;
    }
    return cNew;*/
    throw new NotImplementedException();
  }
  #endregion
}

