using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;

namespace Birds.src.collision.bounding_areas;

public class BoundingAreaFactory
{
  public static readonly ThreadLocal<Stack<BoundingCircle>> circles =
      new(() => new Stack<BoundingCircle>());
  public static readonly ThreadLocal<Stack<AxisAlignedBoundingBox>> AABBs =
      new(() => new Stack<AxisAlignedBoundingBox>());
  public static readonly ThreadLocal<Stack<OrientedBoundingBox>> OBBs =
      new(() => new Stack<OrientedBoundingBox>());

  public static BoundingCircle GetCircle(Vector2 position, float radius)
  {
    var stack = circles.Value;
    if (stack.Count == 0)
      return new BoundingCircle(position, radius);
    else
    {
      BoundingCircle circle = stack.Pop();
      circle.Position = position;
      circle.Radius = radius;
      return circle;
    }
  }

  public static AxisAlignedBoundingBox GetAABB(Vector2 upperLeftCorner, int width, int height)
  {
    var stack = AABBs.Value;
    if (stack.Count == 0)
      return new AxisAlignedBoundingBox(upperLeftCorner, width, height);
    else
    {
      AxisAlignedBoundingBox AABB = stack.Pop();
      AABB.SetBox(upperLeftCorner, width, height);
      return AABB;
    }
  }

  public static OrientedBoundingBox GetOBB(Vector2 upperLeftCorner, float rotation, int width, int height)
  {
    var stack = OBBs.Value;
    if (stack.Count == 0)
      return new OrientedBoundingBox(upperLeftCorner, rotation, width, height);
    else
    {
      OrientedBoundingBox OBB = stack.Pop();
      OBB.SetBox(upperLeftCorner, rotation, width, height);
      return OBB;
    }
  }
}
