using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Birds.src.bounding_areas;
using Birds.src.BVH;
using Birds.src.controllers.modules;
using Birds.src.controllers.modules.steering;
using Birds.src.entities;
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
    private ID_OTHER team;
    public ID_OTHER Team { get { return team; } set { team = value; foreach (IEntity c in Entities) c.Team = value; } }
    public float Radius { get { return radius; } protected set { radius = value; BoundingCircle.Radius = value; } }
    protected float radius;
    //public List<Controller> SeperatedEntities;
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
    public virtual float Rotation{
      set{
        foreach(IEntity e in Entities)
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
    private Color color;
    public Color Color { set { foreach (IEntity c in Entities) c.Color = value; color = value; } get { return color; } }
    public IBoundingArea BoundingArea { get { return BoundingCircle; } }
    public bool IsCollidable { get; set; } = true;
    public SteeringModule Steering { get; set; }

    #endregion
    #region Constructors
    public Controller(List<IEntity> controllables, ID_OTHER team = ID_OTHER.TEAM_AI)
    {
      BoundingCircle = new BoundingCircle(Position, Radius);
      CollisionManager = new AABBTree();
      SetEntities(controllables);
      Team = team;
      //SeperatedEntities = new List<Controller>();
      Color = Color.White;
    }
    public Controller([OptionalAttribute] Vector2 position, ID_OTHER team = ID_OTHER.TEAM_AI)
    {
      BoundingCircle = new BoundingCircle(Position, Radius);
      CollisionManager = new AABBTree();
      Entities = new();
      if (position == null)
        position = Vector2.Zero;
      //SetControllables(new List<IEntity>() { new WorldEntity(position) });
      Team = team;
      //SeperatedEntities = new List<Controller>();
      Color = Color.White;
    }

    #endregion
    #region Methods
    public virtual void Update(GameTime gameTime)
    {
      Steer(gameTime);
      ApplyCohesion(gameTime);
      UpdateEntities(gameTime);
      UpdatePosition();
      UpdateRadius();
      CollisionManager.Update(gameTime);
    }

    protected void ApplyCohesion(GameTime gameTime)
    {
      if (CohesionModule != null)
      {
        CohesionModule.Update(gameTime);
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
      if (newEntities != null)
      {
        List<IEntity> oldControllables = Entities;
        entities = new List<IEntity>();
        foreach (IEntity c in newEntities)
          AddEntity(c);
        if (Entities.Count == 0)
        {
          Entities = oldControllables;
        }
        else
        {
          CollisionManager.UpdateTree(newEntities.Cast<ICollidable>().ToList());
        }
      }
    }
    public virtual void AddEntity(IEntity c)
    {
      if (entities == null)
      {
        entities = new List<IEntity>();
        c.Position = Position;
      }

      if (c != null)
      {
        entities.Add(c);
        UpdatePosition();
        UpdateRadius();
        CollisionManager.Add(c);
        c.Manager = this;
        c.Team = Team;
      }
    }
    private void ApplyInternalGravityN()
    {
      Vector2 distanceFromController;
      foreach (IEntity entity in entities)
      {
        distanceFromController = Position - entity.Position;
        if (distanceFromController.Length() > entity.Radius)
          entity.Accelerate(Vector2.Normalize(Position - entity.Position), Game1.GRAVITY * (Mass - entity.Mass) * entity.Mass / (float)Math.Pow((distanceFromController.Length()), 1)); //2d gravity r is raised to 1
                                                                                                                                                                                        //entity.Accelerate(Vector2.Normalize(Position - entity.Position), (float)Math.Pow(((distanceFromController.Length() - entity.Radius) / AverageDistance()) / 2 * entity.Mass, 2));
      }
    }
    private void ApplyInternalGravityN2()
    {
      foreach (IEntity we1 in entities)
        foreach (IEntity we2 in entities)
          if (we1 != we2)
            we1.AccelerateTo(we2.Position, Game1.GRAVITY * we1.Mass * we2.Mass / (float)Math.Pow(((we1.Position - we2.Position).Length()), 1));
    }
    /**private void CollideProjectiles(IControllable collidable)
    {
      foreach (IControllable c in Controllables)
        if (c is Controller cc)
          cc.CollideProjectiles(collidable);
        else if (c is EntityController ec)
          ec.CollideProjectiles(collidable);
    }*/
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
    public void AccelerateTo(Vector2 position, float thrust)
    {
        foreach(IEntity c in Entities)
        {
            c.AccelerateTo(position, thrust);
        }
    }
    protected void InternalCollission()
    {
      CollisionManager.GetInternalCollissions();
      CollisionManager.ResolveCollissions();
    }
    protected void UpdateEntities(GameTime gameTime)
    {
      foreach (IEntity c in Entities)
        c.Update(gameTime);
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
        BoundingCircle.Position = position;
      }
    }
    public void Draw(SpriteBatch sb)
    {
      foreach (IEntity c in Entities)
        c.Draw(sb);
    }
    public virtual void RotateTo(Vector2 position)
    {
      foreach (IEntity c in Entities)
        c.RotateTo(position);
    }

    public bool CollidesWith(ICollidable otherEntity)
    {
        if(IsCollidable && otherEntity.IsCollidable)
        {
            if (otherEntity is Controller c)
                return c.BoundingCircle.CollidesWith(BoundingCircle);
            else
                throw new Exception("not supported type");
        }
        else
        {
            return false;
        }
    }

    public void Collide(ICollidable otherEntity)
    {
      if (otherEntity is Controller c)
        CollisionManager.CollideWithTree(c.CollisionManager);
      else
        throw new Exception("not supported type");
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