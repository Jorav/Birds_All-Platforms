using Birds.src.containers.controller;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;

namespace Birds.src.utility;

public class Camera
{
  public Matrix Transform { get; private set; }
  public Vector2 Position { get; set; }
  public Vector2 PreviousPosition { get; set; }
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
  public float Width { get { return Game1.ScreenWidth / Zoom; } }
  public float Height { get { return Game1.ScreenHeight / Zoom; } }
  public bool AutoAdjustZoom { get; set; }
  public float GameZoom { get { if (Controller != null) return Math.Min(Game1.ScreenWidth, Game1.ScreenHeight) / (900 + 1 * Controller.Radius); else return 1; } }
  //        public float GameZoom { get { if (Controller != null) return  Game1.ScreenWidth / 3 / Controller.Radius; else return 1; } }
  private Controller controller;
  public Controller Controller { get { return controller; } set { if (value != null) { Position = value.Position; PreviousPosition = value.Position; } controller = value; } }
  private float zoomSpeed;
  private float maxZoom = 3;
  private float minZoom = 0.5f;

  public Camera([OptionalAttribute] Controller controller, float zoomSpeed = 0.01f)
  {
    if (controller != null)
      Controller = controller;
    else
      Position = Vector2.Zero;
    PreviousPosition = Position;
    Rotation = 0;
    Zoom = GameZoom;
    this.zoomSpeed = zoomSpeed;
    AutoAdjustZoom = true;
    UpdateTransformMatrix();
  }

  public void Update()
  {
    PreviousPosition = Position;
    if (Controller != null)
      AdjustPosition();
    if (AutoAdjustZoom)
    {
      AdjustZoom(GameZoom);
    }

    Rotation = 0;
    UpdateTransformMatrix();
  }

  private void AdjustPosition()
  {
    PreviousPosition = Position;
    Position = Controller.Position;// reviousPosition + 0.1f * (Controller.Position - PreviousPosition);
  }

  private void AdjustZoom(float optimalZoom)
  {
    if (optimalZoom > Zoom)
    {
      if (optimalZoom / Zoom > 1 + zoomSpeed)
        Zoom *= 1 + zoomSpeed;
      else
        Zoom = optimalZoom;
    }
    else if (optimalZoom < Zoom)
    {
      if (Zoom / optimalZoom > 1 + zoomSpeed)
        Zoom /= 1 + zoomSpeed;
      else
        Zoom = optimalZoom;
    }
  }

  public Vector2 ScreenToWorld(Vector2 screenPosition)
  {
    float x = (screenPosition.X - (Game1.ScreenWidth / 2)) / Zoom + Position.X;
    float y = (screenPosition.Y - (Game1.ScreenHeight / 2)) / Zoom + Position.Y;

    return new Vector2(x, y);
  }

  public void UpdateTransformMatrix()
  {
    Matrix position = Matrix.CreateTranslation(
        -Position.X,
        -Position.Y,
        0);
    Matrix rotation = Matrix.CreateRotationZ(Rotation);
    Matrix origin = Matrix.CreateTranslation(
        Game1.ScreenWidth / 2,
        Game1.ScreenHeight / 2,
        0);
    Matrix zoom = Matrix.CreateScale(Zoom, Zoom, 0);
    Transform = position * rotation * zoom * origin;
  }
}

