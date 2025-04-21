using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Birds.src.bounding_areas;
using Birds.src.BVH;
using Birds.src.controllers.steering;
using Birds.src.entities;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.controllers
{
  public class Controller : ICollidable
  {
    #region Attributes
    protected List<IEntity> entities;
    public List<IEntity> Entities { get { return entities; } set { SetEntities(value); } }
    public BoundingCircle BoundingCircle { get; set; }
    private AABBTree collisionManager;
    protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
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
    public bool IsCollidable { get; set; }
    public Steering Steering { get; set; }

    #endregion
    #region Constructors
    public Controller(List<IEntity> controllables, ID_OTHER team = ID_OTHER.TEAM_AI)
    {
      BoundingCircle = new BoundingCircle(Position, Radius);
      collisionManager = new AABBTree();
      SetEntities(controllables);
      Team = team;
      //SeperatedEntities = new List<Controller>();
      Color = Color.White;
    }
    public Controller([OptionalAttribute] Vector2 position, ID_OTHER team = ID_OTHER.TEAM_AI)
    {
      BoundingCircle = new BoundingCircle(Position, Radius);
      collisionManager = new AABBTree();
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
      UpdateControllable(gameTime);
      //RemoveEmptyControllers();
      //AddSeperatedEntities();
      UpdatePosition();
      UpdateRadius();
      //ApplyInternalGravityN2();
      collisionManager.Update(gameTime);
      //InternalCollission();
    }

    private void Steer(GameTime gameTime)
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
          AddControllable(c);
        if (Entities.Count == 0)
        {
          Entities = oldControllables;
        }
        else
        {
          collisionManager.UpdateTree(newEntities.Cast<ICollidable>().ToList());
        }
      }
    }
    public virtual void AddControllable(IEntity c)
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
        collisionManager.Add(c);
        c.Manager = this;
        c.Team = team;
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
    /**protected void RemoveEmptyControllers()
      {
          List<IEntity> toBeRemoved = new List<IEntity>();
          foreach (IEntity c in Controllables)
              if (c is WorldEntity we  !we.IsAlive)
                  toBeRemoved.Add(we);
              else if (c is EntityController ec  (ec.Controllables.Count == 0 || !ec.IsAlive))
                  toBeRemoved.Add(ec);
              else if (c is Controller cc  Controllables.Count == 0)
                  toBeRemoved.Add(cc);
          foreach (IEntity c in toBeRemoved)
              Remove(c);
      }*/
    /**protected virtual void AddSeperatedEntities()
      {
          List<EntityController> seperatedEntities = new List<EntityController>();
          foreach (IEntity c in Controllables)
              if (c is EntityController ec)
                  foreach (EntityController ecSeperated in ec.SeperatedEntities)
                  {
                      if (ecSeperated.Controllables.Count == 1  !(ecSeperated.Controllables[0] is Composite))
                          ;//((WorldEntity)(ecSeperated.Controllables[0])).Die();
                      else
                          seperatedEntities.Add(ecSeperated);
                  }
          foreach (EntityController ec in seperatedEntities)
          {
              Controller c = (Controller)Clone();
              c.Controllables.Clear();
              c.AddControllable(ec);
              this.SeperatedEntities.Add(c);
          }

          foreach (IEntity c in Controllables)
              if (c is EntityController ec)
                  ec.SeperatedEntities.Clear();
      }*/
    /**public List<Controller> ExtractAllSeperatedEntities()
      {
          List<Controller> temp = new List<Controller>(SeperatedEntities);
          SeperatedEntities.Clear();
          foreach (IEntity c in Controllables)
          {
              if (c is Controller cc)
                  temp.AddRange(cc.ExtractAllSeperatedEntities());
          }
          return temp;
      }*/
    /**public virtual void Collide(IEntity collidable) // OBS - THIS NEEDS TO BE ADAPTED FOR ICOLLIDABLE
    {
      if (CollidesWith(collidable))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
        foreach (IEntity c in Controllables)
          c.Collide(collidable);
      CollideProjectiles(collidable);
    }*/
    /**private void CollideProjectiles(IControllable collidable)
    {
      foreach (IControllable c in Controllables)
        if (c is Controller cc)
          cc.CollideProjectiles(collidable);
        else if (c is EntityController ec)
          ec.CollideProjectiles(collidable);
    }*/
    /**public bool CollidesWith(IIntersectable c)
    {
      if (c is Controller)
        return collisionDetector.CollidesWith(((Controller)c).collisionDetector);
      else if (c is EntityController)
        return collisionDetector.CollidesWith(((EntityController)c).collisionDetector);
      if (c is WorldEntity && collisionDetector.CollidesWith(((WorldEntity)c).collisionDetector))
        foreach (WorldEntity e in Controllables)
          if (e.CollidesWith((WorldEntity)c))
            return true;
      return false;
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
    protected void InternalCollission()
    {
      collisionManager.GetInternalCollissions();
      collisionManager.ResolveCollissions();
    }
    private void UpdateControllable(GameTime gameTime)
    {
      foreach (IEntity c in Entities)
        c.Update(gameTime);
    }
    protected void UpdateRadius() //TODO: change this to be more exact for composite entities or, more likely, replace it with an AABBTree wrapping all controllers (long term)
    {
      if (Entities.Count == 1)
      {
        if (Entities[0] != null)
          Radius = Entities[0].Radius;
      }
      else if (Entities.Count > 1)
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
    public void RotateTo(Vector2 position)
    {
      foreach (IEntity c in Entities)
        c.RotateTo(position);
    }
    protected float AverageDistance()
    {
      float nr = 1;
      float distance = 0;
      float mass = 0;
      foreach (IEntity c in entities)
      {
        distance += (Vector2.Distance(c.Position, Position) + c.Radius) * c.Mass;
        //nr += 1;
        mass += c.Mass;
      }
      if (mass != 0)
        return distance / nr / mass;
      return 1;
    }

    public bool CollidesWith(ICollidable otherEntity)
    {
      if (otherEntity is Controller c)
        return CollidesWith(c);
      else
        throw new Exception("not supported type");
    }
    public bool CollidesWith(Controller controller)
    {
      return controller.BoundingCircle.CollidesWith(BoundingCircle);
    }

    public void Collide(ICollidable otherEntity)
    {
      if (otherEntity is Controller c)
        Collide(c);
      else
        throw new Exception("not supported type");
    }

    public void Collide(Controller controller)
    {
      collisionManager.CollideWithTree(controller.collisionManager);
    }

    public static String GetName()
    {
      return "No controller";
    }

    public virtual object Clone()
    {
      Controller cNew = (Controller)this.MemberwiseClone();
      cNew.collisionManager = new AABBTree();
      List<IEntity> entities = new List<IEntity>();
      foreach (IEntity c in Entities)
        entities.Add((IEntity)c.Clone());
      cNew.SetEntities(entities);
      cNew.BoundingCircle = BoundingAreaFactory.GetCircle(cNew.Position, cNew.radius);
      cNew.Steering = (Steering)Steering.Clone();
      cNew.Steering.controller = cNew;
      return cNew;
    }
    #endregion
  }

}