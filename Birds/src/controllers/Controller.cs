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
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.controllers
{
  public class Controller : IController
  {
    #region Attributes
    protected List<IEntity> entities;
    public List<IEntity> Entities { get { return entities; } set { SetEntities(value); } }
    public BoundingCircle BoundingCircle { get; private set; }
    public AABBTree CollisionManager { get; protected set; }
    public CohesionModule CohesionModule { get; set; }
    public MovementModule MovementModule { get; set; }
    public SteeringModule Steering { get; set; }

    private ID_OTHER team;
    public ID_OTHER Team { get { return team; } set { team = value; foreach (IEntity entity in Entities) entity.Team = value; } }
    public float Radius { get { return radius; } protected set { radius = value; BoundingCircle.Radius = value; } }
    protected float radius;
    protected Vector2 position;
    public virtual Vector2 Position
    {
      get { return position; }
      set
      {
        Vector2 posChange = value - Position;
        foreach (IEntity c in Entities)
          c.Position += posChange;
        position = value;
        BoundingCircle.Position = value;
      }
    }
    public virtual float Rotation
    {
      set
      {
        foreach (IEntity e in Entities)
          e.Rotation = value;
      }
    }
    public float Mass
    {
      get
      {
        float sum = 0;
        foreach (IEntity c in Entities)
          sum += c.Mass;
        return sum;
      }
    }
    public Color Color { set { foreach (IEntity c in Entities) c.Color = value; color = value; } get { return color; } }
    private Color color;

    public IBoundingArea BoundingArea { get { return BoundingCircle; } }
    public bool IsCollidable { get; set; } = true;

    #endregion
    #region Constructors
    public Controller(List<IEntity> controllables, ID_OTHER team = ID_OTHER.TEAM_AI) : this()
    {
      SetEntities(controllables);
      Team = team;
    }
    public Controller()
    {
      position = Vector2.Zero;
      BoundingCircle = new BoundingCircle(Position, Radius);
      CollisionManager = new AABBTree();
      entities = new List<IEntity>();
      Color = Color.White;
    }

    #endregion
    #region Methods
    public virtual void Update(GameTime gameTime)
    {
      UpdatePosition();
      UpdateRadius();
      Steer(gameTime);
      ApplyMovement(gameTime);
      CohesionModule?.Update(gameTime);
      UpdateEntities(gameTime);
      CollisionManager.Update(gameTime);
    }

    protected void ApplyMovement(GameTime gameTime)
    {
      if (MovementModule != null)
      {
        MovementModule.Update(gameTime);
        Position = MovementModule.Position;
      }
    }

    protected void Steer(GameTime gameTime)
    {
      if (Steering != null)
      {
        Steering.Update(gameTime);
      }
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
      UpdatePosition();
      UpdateRadius();
      CollisionManager.UpdateTree(newEntities.Cast<ICollidable>().ToList());
    }

    public virtual void AddEntity(IEntity e)
    {
      AddEntityBasic(e);
      UpdatePosition();
      UpdateRadius();
    }

    protected void AddEntityBasic(IEntity e)
    {
      if (e == null)
      {
        return;
      }
      entities.Add(e);
      CollisionManager.Add(e);
      e.Manager = this;
      e.Team = Team;
    }

    public void Accelerate(Vector2 directionalVector, float thrust)
    {
      foreach (IEntity c in Entities)
        c.Accelerate(directionalVector, thrust);
    }
    public void Accelerate(Vector2 directionalVector)
    {
      foreach (IEntity c in Entities)
        c.Accelerate(directionalVector);
    }
    protected void UpdateEntities(GameTime gameTime)
    {
      foreach (IEntity c in Entities)
      {
        c.Update(gameTime);
      }
    }
    protected void UpdateRadius() //TODO: change this to be more exact for composite entities or, more likely, replace it with an AABBTree wrapping all controllers (long term)
    {
      float largestDistance = 0;
      foreach (IEntity c in Entities)
      {
        float distance = Vector2.Distance(c.Position, Position) + c.Radius;
        if (distance > largestDistance)
          largestDistance = distance;
      }
      Radius = largestDistance;
    }

    /*
    * Position = mass center
    */
    protected void UpdatePosition() //TODO: only allow IsCollidable to affect this?
    {
      Vector2 sum = Vector2.Zero;
      float weight = 0;
      foreach (IEntity c in Entities)
      {
        weight += c.Mass;
        sum += c.Position * c.Mass;
      }
      if (weight > 0)
      {
        position = sum / (weight);
      }
      else
      {
        position = Vector2.Zero;
      }
      BoundingCircle.Position = position;
    }
    public void Draw(SpriteBatch sb)
    {
      foreach (IEntity c in Entities)
        c.Draw(sb);
    }
    public virtual void RotateTo(Vector2 position)
    {
      foreach (IEntity entity in Entities)
        entity.RotateTo(position);
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
      return cNew;
    }
    #endregion
  }

}