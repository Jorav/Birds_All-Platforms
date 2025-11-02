using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Birds.src.collision.bounding_areas;

public interface IRectangle
{
  Vector2 UL { get; set; }
  Vector2 DL { get; set; }
  Vector2 DR { get; set; }
  Vector2 UR { get; set; }
  public Vector2[] Axes { get; set; }
  public Vector2[] GenerateAxes()
  {
    Axes[0] = new Vector2(UR.X - UL.X, UR.Y - UL.Y);
    Axes[1] = new Vector2(UR.X - DR.X, UR.Y - DR.Y);
    return Axes;
  }
  public static bool CollidesWith(IRectangle r1, IRectangle r2)
  {
    bool collides = true;
    r1.GenerateAxes();
    r2.GenerateAxes();
    r1.Axes = new Vector2[] { r1.Axes[0], r1.Axes[1], r2.Axes[0], r2.Axes[1] };
    float[] scalarA = new float[4];
    float[] scalarB = new float[4];
    foreach (Vector2 axis in r1.Axes)
    {
      scalarA[0] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r1.UL, axis) / axis.LengthSquared()));
      scalarA[1] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r1.DL, axis) / axis.LengthSquared()));
      scalarA[2] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r1.DR, axis) / axis.LengthSquared()));
      scalarA[3] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r1.UR, axis) / axis.LengthSquared()));
      scalarB[0] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r2.UL, axis) / axis.LengthSquared()));
      scalarB[1] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r2.DL, axis) / axis.LengthSquared()));
      scalarB[2] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r2.DR, axis) / axis.LengthSquared()));
      scalarB[3] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r2.UR, axis) / axis.LengthSquared()));
      if (scalarB.Max() < scalarA.Min() + 0.1f || scalarA.Max() < scalarB.Min() + 0.1f)
        collides = false;
    }
    return collides;
  }
}