using Birds.src;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

public class Camera
{
  public Matrix Transform { get; private set; }
  public Vector2 Position { get; set; }
  public Vector2 PreviousPosition { get; set; }
  public bool IsLocked { get; set; }
  public float Rotation { get; set; }

  private float zoom;
  public float Zoom
  {
    get { return zoom; }
    set
    {
      if (value > maxZoom)
        value = maxZoom;
      else if (value < minZoom)
        value = minZoom;
      zoom = value;
    }
  }

  private bool inBuildScreen;
  public bool InBuildScreen
  {
    get { return inBuildScreen; }
    set
    {
      if (value)
      {
        Zoom = BuildMenuZoom;
      }
      else
      {
        Zoom = GameZoom;
      }
      inBuildScreen = value;
      UpdateTransformMatrix();
    }
  }
  public float Width { get { return Game1.ScreenWidth / Zoom; } }
  public float Height { get { return Game1.ScreenHeight / Zoom; } }
  public bool AutoAdjustZoom { get; set; }

  private IModuleContainer controller;
  public IModuleContainer Controller
  {
    get { return controller; }
    set
    {
      if (value != null)
      {
        Position = value.Position;
        PreviousPosition = value.Position;
      }
      controller = value;
    }
  }
  public float GameZoom
  {
    get
    {
      if (Controller != null)
        return Math.Min(Game1.ScreenWidth, Game1.ScreenHeight) / (900 + 1 * Controller.Radius);
      else
        return 1;
    }
  }

  public float BuildMenuZoom
  {
    get
    {
      if (Controller != null)
        return Math.Min(Game1.ScreenWidth, Game1.ScreenHeight) / (2 * Controller.Radius + 900 / 8);
      else
        return 1;
    }
  }

  private float maxZoom = 6;
  private float minZoom = 0.5f;

  public Camera(IModuleContainer controller = null)
  {
    Controller = controller;
    Position = controller?.Position ?? Vector2.Zero;
    PreviousPosition = Position;
    Rotation = 0;
    Zoom = 1;
    AutoAdjustZoom = true;
    UpdateTransformMatrix();
  }

  public Vector2 ScreenToWorld(Vector2 screenPosition)
  {
    float x = (screenPosition.X - (Game1.ScreenWidth / 2)) / Zoom + Position.X;
    float y = (screenPosition.Y - (Game1.ScreenHeight / 2)) / Zoom + Position.Y;
    return new Vector2(x, y);
  }

  public void UpdateTransformMatrix()
  {
    Matrix position = Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
    Matrix rotation = Matrix.CreateRotationZ(Rotation);
    Matrix origin = Matrix.CreateTranslation(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2, 0);
    Matrix zoom = Matrix.CreateScale(Zoom, Zoom, 0);
    Transform = position * rotation * zoom * origin;
  }
}