

using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Birds.src.bounding_areas
{
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
    public bool CollidesWith(IBoundingArea boundingArea)
    {
      if (boundingArea is BoundingCircle boundingCircle)
      {
        return CollidesWith(boundingCircle);
      }
      else
        throw new Exception("comparing different boundingarea types (that are not currently supported)");
    }

    public Vector2 CalculateOverlapRepulsion(BoundingCircle c)
    {
      float distance2 = (Position - c.Position).Length();
      if (distance2 < 5)
        distance2 = 5;
      return 30f * Vector2.Normalize(Position - c.Position) / distance2;
    }

    public void Deprecate()
    {
      BoundingAreaFactory.circles.Append(this);
    }
  }
}
