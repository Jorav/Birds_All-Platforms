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

namespace Birds.src.entities
{
  public class WorldEntity : ModuleContainerBase, IEntity
  {
    #region Properties
    //public float Scale { get { return sprite.Scale; } set { sprite.Scale = value; /*BoundingArea.Scale = value; oldCollisionDetector.Scale = value;*/ foreach (Link l in Links) l.Scale = value;/*add collisionDetector scale in the future*/ } }
    protected Vector2 origin;
    public List<Link> Links { get; private set; } = new();
    private float internalRotation;
    public bool IsFiller { get; set; }
    public ID_ENTITY EntityID { get; set; }
    public Controller Manager { get; set; }
    #endregion
    public WorldEntity()
    {
    }
    #region Methods
    public void Draw(SpriteBatch sb)
    {
      GetModule<SpriteModule>()?.Draw(sb);
    }

    public virtual object Clone()
    {/**
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
      return eNew;*/
      throw new NotImplementedException();
    }
    #endregion
    #region Linking
    public virtual void AddLinks()
    {
      if (Links.Count > 0)
        Links.Clear();

      // Get dimensions from sprite module
      var spriteModule = GetModule<SpriteModule>();
      if (spriteModule != null)
      {
        float width = spriteModule.Sprite.Width;

        Links.Add(new Link(new Vector2(0, -width / 2), this));
        Links.Add(new Link(new Vector2(width / 2, 0), this));
        Links.Add(new Link(new Vector2(0, width / 2), this));
        Links.Add(new Link(new Vector2(-width / 2, 0), this));
      }
    }

    public void ConnectTo(WorldEntity eConnectedTo, Link lConnectedTo)
    {
      if (Links.Count > 0 && Links[0] != null)
      {
        lConnectedTo.SeverConnection();
        internalRotation = Links[0].ConnectTo(lConnectedTo);
        Rotation.Value = eConnectedTo.Rotation - eConnectedTo.internalRotation;
        Position.Value = lConnectedTo.ConnectionPosition;
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
