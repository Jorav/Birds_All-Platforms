using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.factories;
using Birds.src.modules.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Birds.src.entities
{
  public class WorldEntity : IEntity
  {
    #region Properties
    public EntityMovementModule MovementModule { get; private set; }
    protected Sprite sprite = null;
    private OrientedBoundingBox OBB;
    public IBoundingArea BoundingArea { get { return OBB; } }
    public BoundingCircle BoundingCircle { get; private set; }
    public Vector2 Position
    {
      get { return MovementModule.Position; }
      set
      {
        MovementModule.Position = value;
        sprite.Position = value;
        OBB.Position = value;
        BoundingCircle.Position = value;
      }
    }
    public float Rotation
    {
      get { return MovementModule.Rotation; }
      set
      {
        MovementModule.Rotation = value;
        sprite.Rotation = value;
        OBB.Rotation = value;
      }
    }
    public bool IsVisible { get { return sprite.isVisible; } set { sprite.isVisible = value; } }
    public float Width { get { return sprite.Width; } }
    public float Height { get { return sprite.Height; } }
    public float Radius { get { return BoundingCircle.Radius; } }
    public float Scale { get { return sprite.Scale; } set { sprite.Scale = value; /*BoundingArea.Scale = value; oldCollisionDetector.Scale = value;*/ foreach (Link l in Links) l.Scale = value;/*add collisionDetector scale in the future*/ } }
    public Vector2 Origin
    {
      get { return origin; }
      set
      {
        origin = value;
        sprite.Origin = value;
      }
    }
    protected Vector2 origin;
    public List<Link> Links { get; private set; }
    private float internalRotation;
    public bool IsFiller { get; set; }
    public bool IsCollidable { get; set; }
    public ID_ENTITY EntityID { get; set; }
    public ID_OTHER Team { get; set; }
    public Controller Manager { get; set; }
    public Color Color { get; set; }
    #endregion
    public WorldEntity()
    {
    }
    #region Methods
    public bool AddModule(Object o)
    {
      switch (o)
      {
        case EntityMovementModule m: MovementModule = m; break;
        case Sprite s: sprite = s; break;
        case OrientedBoundingBox obb: OBB = obb; break;
        case BoundingCircle bc: BoundingCircle = bc; break;
        default: return false;
      }
      return true;
    }
    public void SetAttributes(ID_ENTITY id, Vector2 position, float rotation, float mass, float thrust, float friction, bool isVisible, bool isCollidable, float scale)
    {
      MovementModule = MovementModuleFactory.GetMovementModule(position, rotation, mass, thrust, friction);
      sprite = SpriteFactory.GetSprite(id, position, scale);
      OBB = BoundingAreaFactory.GetOBB(position, rotation, sprite.Width, sprite.Height);
      BoundingCircle = BoundingAreaFactory.GetCircle(position, OBB.Radius);
      Position = position;
      Rotation = rotation;
      IsVisible = isVisible;
      IsCollidable = isCollidable;
      Links = new List<Link>();
      AddLinks();
      Origin = new Vector2(Width / 2, Height / 2);
      EntityID = id;
    }
    public void Draw(SpriteBatch sb)
    {
      sprite.Draw(sb);
    }
    public void Collide(IEntity e)
    {
      MovementModule.TotalExteriorForce += MovementModule.CalculateCollissionRepulsion(e.MovementModule) + BoundingCircle.CalculateOverlapRepulsion(e.BoundingCircle);
    }

    public void Collide(ICollidable c)
    {
      if (c is IEntity e)
        Collide(e);
      else
        throw new Exception("not implemented for other types");
    }

    public bool CollidesWith(ICollidable collidable)
    {
      if (IsCollidable && collidable.IsCollidable)
      {
        if (collidable is IEntity entity)
        {
          if (entity.BoundingCircle.CollidesWith(BoundingCircle))
            return BoundingArea.CollidesWith(collidable.BoundingArea);
          else
            return false;
        }
        else
          return BoundingArea.CollidesWith(collidable.BoundingArea);
      }
      else
        return false;
    }

    public void Update(GameTime gameTime)
    {
      MovementModule.Update2(gameTime);
      Rotation = MovementModule.Rotation;
      Position = Position;
      /*
        if (Health <= 0)
          Die();
      */
    }

    public virtual object Clone()
    {
      WorldEntity eNew = EntityFactory.GetEntity(Position, EntityID);
      eNew.Color = Color;
      eNew.Scale = Scale;
      eNew.MovementModule.Friction = MovementModule.Friction;
      eNew.IsCollidable = IsCollidable;
      eNew.IsFiller = IsFiller;
      eNew.IsVisible = IsVisible;
      eNew.Manager = Manager;
      eNew.MovementModule.Mass = MovementModule.Mass;
      eNew.Origin = Origin;
      eNew.Rotation = Rotation;
      eNew.Team = Team;
      eNew.MovementModule.Thrust = MovementModule.Thrust;
      eNew.MovementModule.Velocity = MovementModule.Velocity;
      eNew.internalRotation = Rotation;
      eNew.MovementModule.Velocity = Vector2.Zero;
      eNew.MovementModule.TotalExteriorForce = Vector2.Zero;
      eNew.Links = new List<Link>();
      eNew.AddLinks();
      return eNew;
    }
    #endregion
    #region Linking
    protected virtual void AddLinks()
    {
      if (Links.Count > 0)
        Links.Clear();

      Links.Add(new Link(new Vector2(0, -Width / 2), this));
      Links.Add(new Link(new Vector2(Width / 2, 0), this));
      Links.Add(new Link(new Vector2(0, Width / 2), this));
      Links.Add(new Link(new Vector2(-Width / 2, 0), this));
    }

    public void ConnectTo(WorldEntity eConnectedTo, Link lConnectedTo)
    {
      if (Links.Count > 0 && Links[0] != null)
      {
        lConnectedTo.SeverConnection();
        internalRotation = Links[0].ConnectTo(lConnectedTo);
        Rotation = eConnectedTo.Rotation - eConnectedTo.internalRotation;
        Position = lConnectedTo.ConnectionPosition;
      }
    }
    /*
    public List<WorldEntity> FillEmptyLinks()
    {
        List<WorldEntity> entities = new List<WorldEntity>();
        IDs fillerType = IDs.EMPTY_LINK;
        foreach (Link l in Links)
            if (l.ConnectionAvailable)
            {
                WorldEntity e = EntityFactory.Create(position, fillerType);
                e.ConnectTo(this, l);
                e.IsFiller = true;
                entities.Add(e);
            }
        return entities;
    }*/

    public void SeverConnection(WorldEntity e)
    {
      foreach (Link l in Links)
        if (l.ConnectionAvailable && l.connection.Entity == e)
        {
          l.SeverConnection();
        }
    }

    public void Deprecate()
    {
      MovementModule.Deprecate();
      sprite.Deprecate();
      OBB.Deprecate();
      BoundingCircle.Deprecate();
      EntityFactory.availableEntities.Push(this);
    }

    public class Link
    {
      public WorldEntity Entity { get; private set; }
      public Link connection; //links to other entities
      public Vector2 RelativePosition { get; private set; } //position in relation to the entity it belongs to in an unrotated state
      public Vector2 RelativePositionRotated { get { return RelativePosition.Length() * Scale * new Vector2((float)Math.Cos(MathHelper.WrapAngle(LinkRotation + Entity.Rotation)), (float)Math.Sin(MathHelper.WrapAngle(LinkRotation + Entity.Rotation))); } }
      public Vector2 AbsolutePosition { get { return Entity.Position + RelativePositionRotated; } }
      public Vector2 ConnectionPosition { get { Vector2 dir = new Vector2((float)Math.Cos(MathHelper.WrapAngle(LinkRotation + Entity.Rotation)), (float)Math.Sin(MathHelper.WrapAngle(LinkRotation + Entity.Rotation))); if (!ConnectionAvailable) return Entity.Position + DistanceFromConnection * dir; else return Entity.Position + dir * RelativePosition.Length() * Scale * 2; } }
      public float Scale { get; set; }
      public float LinkRotation { get; set; } //rotation of link in relation to center of entity
      public float DistanceFromConnection { get { if (!ConnectionAvailable) return RelativePosition.Length() * Scale + connection.RelativePosition.Length() * connection.Scale; throw new Exception(); } }
      public bool ConnectionAvailable { get { return connection == null; } }

      public Link(Vector2 relativePosition, WorldEntity entity, Link connection = null)
      {
        this.Entity = entity;
        this.RelativePosition = relativePosition;
        this.connection = connection;
        if (relativePosition.Length() != 0)
        {
          if (relativePosition.X >= 0)
            LinkRotation = (float)Math.Atan(relativePosition.Y / relativePosition.X);
          else
            LinkRotation = (float)Math.Atan(relativePosition.Y / relativePosition.X) - MathHelper.ToRadians(180);
        }
        Scale = 1;
      }

      /**
       * returns the internal rotation of the entity it belongs to
       */
      public float ConnectTo(Link l)
      {
        if (!l.ConnectionAvailable)
          l.SeverConnection();
        connection = l;
        l.connection = this;
        return MathHelper.WrapAngle(l.Entity.internalRotation + l.LinkRotation + LinkRotation + MathHelper.ToRadians(180));
        //return MathHelper.ToRadians(180) - MathHelper.WrapAngle(-l.LinkRotation - LinkRotation);
      }

      public void SeverConnection()
      {
        if (!ConnectionAvailable)
        {
          connection.connection = null;
          connection = null;
        }
      }
      /*
      public object Clone()
      {
          Link lNew = (Link)this.MemberwiseClone();
      }*/
    }
    #endregion
  }
}
