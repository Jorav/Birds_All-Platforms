using Birds.src.entities;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.controllers.composite;

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
    if (relativePosition.Length() != 0)
    {
      if (relativePosition.X >= 0)
        LinkRotation = (float)Math.Atan(relativePosition.Y / relativePosition.X);
      else
        LinkRotation = (float)Math.Atan(relativePosition.Y / relativePosition.X) - MathHelper.ToRadians(180);
    }
    Scale = 1;
  }

  public float ConnectTo(Link l)
  {
    if (!l.ConnectionAvailable)
      l.SeverConnection();
    connection = l;
    l.connection = this;

    var otherLinkModule = l.Entity.GetModule<LinkModule>();
    float otherInternalRotation = otherLinkModule?.InternalRotation ?? 0f;

    return MathHelper.WrapAngle(otherInternalRotation + l.LinkRotation + LinkRotation + MathHelper.ToRadians(180));
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
