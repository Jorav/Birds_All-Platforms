using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Birds.src.collision.bounding_areas;
public class BoundingCircle : IBoundingArea
{
  public Vector2 Position { get; set; }
  public float Radius { get; set; }
  public float Area { get { return Radius * Radius; } }
  public (float, float) MaxXY { get { return (Position.X + Radius, Position.Y + Radius); } }
  public (float, float) MinXY { get { return (Position.X - Radius, Position.Y - Radius); } }


  public BoundingCircle(Vector2 position, float radius)
  {
    Position = position;
    Radius = radius;
  }

  public BoundingCircle CombinedBoundingCircle(BoundingCircle cOther)
  {
    BoundingCircle largestCircle;
    BoundingCircle smallestCircle;
    if (Radius > cOther.Radius)
    {
      largestCircle = this;
      smallestCircle = cOther;
    }
    else
    {
      largestCircle = cOther;
      smallestCircle = this;
    }
    Vector2 smallestToLargest = largestCircle.Position - smallestCircle.Position;
    float distance = smallestToLargest.Length();
    smallestToLargest.Normalize();

    if (largestCircle.Radius > distance + smallestCircle.Radius) //if smallest circle completely inside large circle
      return BoundingAreaFactory.GetCircle(largestCircle.Position, largestCircle.Radius);
    else if (distance >= largestCircle.Radius) //if smallest circle center outside large circle
    {
      Vector2 position = (smallestCircle.Position + largestCircle.Position + smallestToLargest * largestCircle.Radius - smallestToLargest * smallestCircle.Radius) / 2;
      float radius = (largestCircle.Position - position).Length() + largestCircle.Radius;
      return BoundingAreaFactory.GetCircle(position, radius);
    }
    else //if smallest circle center inside large circle but not fully
    {
      Vector2 position = (smallestCircle.Position + largestCircle.Position + smallestToLargest * largestCircle.Radius - smallestToLargest * smallestCircle.Radius) / 2;
      float radius = (largestCircle.Position - position).Length() + largestCircle.Radius;
      return BoundingAreaFactory.GetCircle(position, radius);
    }
  }

  public bool CollidesWith(BoundingCircle c)
  {
    return Math.Sqrt(Math.Pow((double)(Position.X) - (double)(c.Position.X), 2) + Math.Pow((double)(Position.Y) - (double)(c.Position.Y), 2)) <= (Radius + c.Radius);
  }

  public bool Contains(Vector2 position)
  {
    return Vector2.DistanceSquared(position, Position) <= Radius * Radius;
  }

  public Vector2 CalculateOverlapRepulsion(BoundingCircle c)
  {
    Vector2 delta = Position - c.Position;
    float distance = delta.Length();
    if (distance < 0.1f)
    {
      distance = 0.1f;
      delta = new Vector2(0.1f, 0.05f);
    }
    float overlap = Radius + c.Radius - distance;
    if (overlap <= 0)
      return Vector2.Zero;
    if (overlap > 5f)
      overlap = 5f;
    return delta/distance * overlap/c.Radius;
  }

  public void Deprecate()
  {
    BoundingAreaFactory.circles.Append(this);
  }
}

