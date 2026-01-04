using Birds.src.containers.entity;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.containers.composite;

public class Link
{
  public IEntity Entity { get; private set; }
  public Link connection;
  public Vector2 RelativePosition { get; private set; }
  public Vector2 RelativePositionRotated {
    get
    { 
      return RelativePosition.Length()* Scale * new Vector2((float)Math.Cos(MathHelper.WrapAngle(LinkRotation + Entity.Rotation.Value)),
        (float)Math.Sin(MathHelper.WrapAngle(LinkRotation + Entity.Rotation.Value))); 
    } 
  }
  public Vector2 AbsolutePosition { get { return Entity.Position.Value + RelativePositionRotated; } }
  public Vector2 ConnectionPosition { 
    get
    { 
      Vector2 dir = new Vector2((float)Math.Cos(MathHelper.WrapAngle(LinkRotation + Entity.Rotation.Value)),
        (float)Math.Sin(MathHelper.WrapAngle(LinkRotation + Entity.Rotation.Value))); 
      if (!ConnectionAvailable)
      {
        return Entity.Position.Value + DistanceFromConnection * dir;
      }
      else
      {
        return Entity.Position.Value + dir * RelativePosition.Length() * Scale * 2;
      }
    }
  }
  public float Scale { get; set; }
  public float LinkRotation { get; set; }
  public float DistanceFromConnection {
    get
    { 
      if (!ConnectionAvailable)
      {
        return RelativePosition.Length() * Scale + connection.RelativePosition.Length() * connection.Scale;
      }
      throw new Exception();
    }
  }
  public bool ConnectionAvailable { get { return connection == null; } }

public Link(Vector2 relativePosition, IEntity entity, Link connection = null)
{
    this.Entity = entity;
    this.RelativePosition = relativePosition;
    this.connection = connection;
    
    LinkRotation = (float)Math.Atan2(relativePosition.Y, relativePosition.X);
    
    Scale = 1;
}

  public void ConnectTo(Link l)
  {
    if (!l.ConnectionAvailable)
      l.SeverConnection();
    connection = l;
    l.connection = this;
  }

  public void SeverConnection()
  {
    if (!ConnectionAvailable)
    {
      connection.connection = null;
      connection = null;
    }
  }
}
