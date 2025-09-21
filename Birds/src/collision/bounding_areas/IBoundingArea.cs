using Microsoft.Xna.Framework;
using System;

namespace Birds.src.collision.bounding_areas;
public interface IBoundingArea
{
  public Vector2 Position { get; set; }
  public float Radius { get; }
  //public float Scale {get; set;}

  //returns a tuple with the maximum X positions and maximum y position of the whole object (these two values does not necessarily belong to the same point)
  public (float, float) MaxXY { get; }

  //returns a tuple with the minimum X positions and minimum y position of the whole object (these two values does not necessarily belong to the same point)
  public (float, float) MinXY { get; }

  public bool Contains(Vector2 position);

  public static bool CollidesWith(IBoundingArea a, IBoundingArea b)
  {
    if (a is BoundingCircle bc1 && b is BoundingCircle bc2)
      return CollidesWith(bc1, bc2);

    if (a is AxisAlignedBoundingBox aabb1 && b is AxisAlignedBoundingBox aabb2)
      return CollidesWith(aabb1, aabb2);

    if (a is OrientedBoundingBox obb1 && b is OrientedBoundingBox obb2)
      return CollidesWith(obb1, obb2);

    if (a is AxisAlignedBoundingBox aabb && b is BoundingCircle bc)
      return CollidesWith(aabb, bc);
    if (a is BoundingCircle bc3 && b is AxisAlignedBoundingBox aabb3)
      return CollidesWith(aabb3, bc3);

    if (a is OrientedBoundingBox obb && b is BoundingCircle bc4)
      return CollidesWith(obb, bc4);
    if (a is BoundingCircle bc5 && b is OrientedBoundingBox obb3)
      return CollidesWith(obb3, bc5);

    if (a is AxisAlignedBoundingBox aabb4 && b is OrientedBoundingBox obb4)
      return CollidesWith(aabb4, obb4);
    if (a is OrientedBoundingBox obb5 && b is AxisAlignedBoundingBox aabb5)
      return CollidesWith(aabb5, obb5);

    throw new NotImplementedException($"Collision detection not implemented for types: {a.GetType().Name} and {b.GetType().Name}");
  }

  public static bool CollidesWith(AxisAlignedBoundingBox aabb, BoundingCircle bc)
  {
    return aabb.CollidesWith(bc);
  }
  public static bool CollidesWith(AxisAlignedBoundingBox aabb, OrientedBoundingBox obb)
  {
    return IRectangle.CollidesWith(aabb, obb);
  }
  public static bool CollidesWith(OrientedBoundingBox obb, BoundingCircle bc)
  {
    return obb.CollidesWith(bc);
  }
  public static bool CollidesWith(AxisAlignedBoundingBox aabb1, AxisAlignedBoundingBox aabb2)
  {
    return aabb1.CollidesWith(aabb2);
  }
  public static bool CollidesWith(OrientedBoundingBox obb1, OrientedBoundingBox obb2)
  {
    return IRectangle.CollidesWith(obb1, obb2);
  }
  public static bool CollidesWith(BoundingCircle bc1, BoundingCircle bc2)
  {
    return bc1.CollidesWith(bc2);
  }

}
