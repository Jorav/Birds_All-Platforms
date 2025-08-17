using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Birds.src.bounding_areas
{
  public class OrientedBoundingBox : IBoundingArea
  {
    private Vector2 UL { get; set; }
    private Vector2 DL { get; set; }
    private Vector2 DR { get; set; }
    private Vector2 UR { get; set; }

    private float width;
    private float height;
    private float scale = 1f;

    public float Width
    {
      get { return width; }
      set
      {
        width = value;
        origin = new Vector2(width / 2, height / 2);
        UpdateCorners();
        UpdateRadius();
      }
    }

    public float Height
    {
      get { return height; }
      set
      {
        height = value;
        origin = new Vector2(width / 2, height / 2);
        UpdateCorners();
        UpdateRadius();
      }
    }

    public float Scale
    {
      get { return scale; }
      set
      {
        scale = value;
        UpdateCorners();
        UpdateRadius();
      }
    }

    public Vector2 AbsolutePosition { get { return (UL + DR) / 2; } }
    public (float, float) MaxXY { get; set; }
    public (float, float) MinXY { get; set; }
    private Vector2 origin;
    public Vector2 Origin
    {
      set
      {
        origin = value;
        UpdateCorners();
      }
      get
      {
        return origin;
      }
    }
    private Vector2 position;
    public Vector2 Position
    {
      set
      {
        position = value;
        UpdateCorners();
      }
      get
      {
        return position;
      }
    }
    private float rotation = 0;
    public float Rotation
    {
      set
      {
        rotation = value;
        UpdateCorners();
      }
      get { return rotation; }
    }

    float IBoundingArea.Radius => Radius;

    public float Radius;
    Vector2[] axes = new Vector2[2];

    public OrientedBoundingBox(Vector2 position, float rotation, int width, int height)
    {
      SetBox(position, rotation, width, height);
    }

    public bool CollidesWith(OrientedBoundingBox r)
    {
      bool collides = true;
      GenerateAxes();
      r.GenerateAxes();
      axes = new Vector2[] { axes[0], axes[1], r.axes[0], r.axes[1] };
      float[] scalarA = new float[4];
      float[] scalarB = new float[4];
      foreach (Vector2 axis in axes)
      {
        scalarA[0] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(UL, axis) / axis.LengthSquared()));
        scalarA[1] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(DL, axis) / axis.LengthSquared()));
        scalarA[2] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(DR, axis) / axis.LengthSquared()));
        scalarA[3] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(UR, axis) / axis.LengthSquared()));
        scalarB[0] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r.UL, axis) / axis.LengthSquared()));
        scalarB[1] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r.DL, axis) / axis.LengthSquared()));
        scalarB[2] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r.DR, axis) / axis.LengthSquared()));
        scalarB[3] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r.UR, axis) / axis.LengthSquared()));
        if (scalarB.Max() < scalarA.Min() + 0.01f || scalarA.Max() < scalarB.Min() + 0.01f)
          collides = false;
      }
      return collides;
    }

    private void UpdateCorners()
    {
      Matrix transform = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position.X, position.Y, 0);

      Vector2 topLeft = new Vector2(0, 0) - origin;
      Vector2 topRight = new Vector2(width, 0) - origin;
      Vector2 bottomLeft = new Vector2(0, height) - origin;
      Vector2 bottomRight = new Vector2(width, height) - origin;

      UL = Vector2.Transform(topLeft, transform);
      UR = Vector2.Transform(topRight, transform);
      DL = Vector2.Transform(bottomLeft, transform);
      DR = Vector2.Transform(bottomRight, transform);

      UpdateMaxAndMin();
    }

    private void UpdateRadius()
    {
      Radius = (float)Math.Sqrt(Math.Pow(width * scale / 2, 2) + Math.Pow(height * scale / 2, 2));
    }

    private void UpdateMaxAndMin()
    {
      MaxXY = ((float)Math.Max(Math.Max(UL.X, UR.X), Math.Max(DL.X, DR.X)), (float)Math.Max(Math.Max(UL.Y, UR.Y), Math.Max(DL.Y, DR.Y)));
      MinXY = ((float)Math.Min(Math.Min(UL.X, UR.X), Math.Min(DL.X, DR.X)), (float)Math.Min(Math.Min(UL.Y, UR.Y), Math.Min(DL.Y, DR.Y)));
    }

    public bool Contains(Vector2 position)
    {
      Vector2 AM = position - UL;
      Vector2 AD = DL - UL;
      Vector2 AB = UR - UL;
      return 0 <= Vector2.Dot(AM, AB) && Vector2.Dot(AM, AB) <= Vector2.Dot(AB, AB) && 0 <= Vector2.Dot(AM, AD) && Vector2.Dot(AM, AD) <= Vector2.Dot(AD, AD);
    }
    public Vector2[] GenerateAxes()
    {
      axes[0] = new Vector2(UR.X - UL.X, UR.Y - UL.Y);
      axes[1] = new Vector2(UR.X - DR.X, UR.Y - DR.Y);
      return axes;
    }

    public bool CollidesWith(IBoundingArea boundingArea)
    {
      if (boundingArea is OrientedBoundingBox OBB)
        return CollidesWith(OBB);
      else
        throw new NotImplementedException();
    }

    public void SetBox(Vector2 upperLeftCorner, float rotation, int width, int height)
    {
      this.position = upperLeftCorner;
      this.width = width;
      this.height = height;
      this.rotation = rotation;
      this.scale = 1f;
      origin = new Vector2(width / 2, height / 2);
      UpdateCorners();
      UpdateRadius();
    }

    public void Deprecate()
    {
      BoundingAreaFactory.OBBs.Push(this);
    }
  }
}