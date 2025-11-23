using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Birds.src.collision.bounding_areas;

public class OrientedBoundingBox : IBoundingArea, IRectangle
{
  public Vector2 UL { get; set; }
  public Vector2 DL { get; set; }
  public Vector2 DR { get; set; }
  public Vector2 UR { get; set; }

  private float width;
  private float height;
  private float scale = 1f;

  public float Width
  {
    get { return width; }
    set
    {
      width = value;
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

  public (float, float) MaxXY { get; set; }
  public (float, float) MinXY { get; set; }

  private Vector2 position;
  public Vector2 Position
  {
    get
    {
      return position;
    }
    set
    {
      position = value;
      UpdateCorners();
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
  public Vector2[] Axes { get; set; } = new Vector2[2];

  public OrientedBoundingBox(Vector2 position, float rotation, int width, int height)
  {
    SetBox(position, rotation, width, height);
  }

  private void UpdateCorners()
  {
    Matrix transform = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position.X, position.Y, 0);

    float halfWidth = width / 2;
    float halfHeight = height / 2;

    Vector2 topLeft = new Vector2(-halfWidth, -halfHeight);
    Vector2 topRight = new Vector2(halfWidth, -halfHeight);
    Vector2 bottomLeft = new Vector2(-halfWidth, halfHeight);
    Vector2 bottomRight = new Vector2(halfWidth, halfHeight);

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

  public bool CollidesWith(BoundingCircle circle)
  {
    Matrix inverseTransform =
        Matrix.CreateTranslation(-position.X, -position.Y, 0) *
        Matrix.CreateRotationZ(-rotation) *
        Matrix.CreateScale(1f / scale);

    Vector2 localCirclePos = Vector2.Transform(circle.Position, inverseTransform);
    float halfWidth = width / 2;
    float halfHeight = height / 2;

    float closestX = MathHelper.Clamp(localCirclePos.X, -halfWidth, halfWidth);
    float closestY = MathHelper.Clamp(localCirclePos.Y, -halfHeight, halfHeight);

    float deltaX = localCirclePos.X - closestX;
    float deltaY = localCirclePos.Y - closestY;

    float distanceSquared = deltaX * deltaX + deltaY * deltaY;
    return distanceSquared <= (circle.Radius * circle.Radius);
  }


  public void SetBox(Vector2 centerPosition, float rotation, int width, int height)
  {
    this.position = centerPosition;
    this.width = width;
    this.height = height;
    this.rotation = rotation;
    this.scale = 1f;
    UpdateCorners();
    UpdateRadius();
  }

  public void SetDimensions(float width, float height)
  {
    this.width = width;
    this.height = height;
    UpdateCorners();
    UpdateRadius();
  }

  public void Dispose()
  {
    BoundingAreaFactory.OBBs.Push(this);
  }
}