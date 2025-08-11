using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Birds.src.bounding_areas;
using Birds.src.BVH;
using Birds.src.entities;
using Birds.src.modules.controller;
using Birds.src.modules.controller.steering;
using Birds.src.modules.entity;
using Birds.src.modules.events;
using Birds.src.modules.shared.bounding_area;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.controllers;
public class Controller : ModuleContainerBase, IController
{
  #region Attributes
  protected List<IEntity> entities;
  public List<IEntity> Entities { get { return entities; } set { SetEntities(value); } }
  public BoundingCircle BoundingCircle => GetModule<BCCollisionDetectionModule>()?.BoundingCircle;
  public AABBTree? CollisionManager { get; set; }
  public CohesionModule CohesionModule => GetModule<CohesionModule>();
  public ControllerMovementModule MovementModule => GetModule<ControllerMovementModule>();
  public SteeringModule Steering => GetModule<SteeringModule>();
  public IBoundingArea BoundingArea { get { return BoundingCircle; } }
  public bool IsCollidable { get; set; } = true;

  #endregion
  #region Constructors
  public Controller(List<IEntity> controllables, ID_OTHER team = ID_OTHER.TEAM_AI) : this()
  {
    SetEntities(controllables);
  }
  public Controller() : base()
  {
    CollisionManager = new AABBTree();
    entities = new List<IEntity>();
  }

  #endregion
  #region Methods
  public virtual void Update(GameTime gameTime)
  {
    CollisionManager.Update(gameTime);
    base.Update(gameTime);
  }

  public virtual void SetEntities(List<IEntity> newEntities)
  {
    if (newEntities == null || newEntities.Count == 0)
    {
      return;
    }
    DeprecateEntities();
    foreach (IEntity e in newEntities)
    {
      AddEntityBasic(e);
    }
    CollisionManager.UpdateTree(newEntities.Cast<ICollidable>().ToList());
  }

  public virtual void AddEntity(IEntity e)
  {
    AddEntityBasic(e);
  }

  protected void AddEntityBasic(IEntity e)
  {
    if (e == null)
    {
      return;
    }
    CollisionManager.Add(e);
    e.Manager = this;
    base.Entities.Add(e);
  }
  public void Draw(SpriteBatch sb)
  {
    foreach (IEntity c in Entities)
      c.Draw(sb);
  }

  public bool CollidesWith(ICollidable otherEntity)
  {
    if (!IsCollidable || !otherEntity.IsCollidable)
    {
      return false;
    }
    if (otherEntity is Controller c)
      return c.BoundingCircle.CollidesWith(BoundingCircle);
    else
      throw new Exception("not supported type");

  }

  public void Collide(ICollidable otherEntity)
  {
    if (otherEntity is Controller c)
      CollisionManager.CollideWithTree(c.CollisionManager);
    else
      throw new Exception("not supported type");
  }

  private void DeprecateEntities()
  {
    foreach (IEntity e in Entities)
    {
      e.Deprecate();
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

