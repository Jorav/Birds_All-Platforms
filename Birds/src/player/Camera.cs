using Birds.src;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.player;

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

  private IModuleContainer trackedController;
  public IModuleContainer TrackedController
  {
    get { return trackedController; }
    set
    {
      if (value != null)
      {
        Position = value.Position;
        PreviousPosition = value.Position;
      }
      trackedController = value;
    }
  }

  public float GameZoom
  {
    get
    {
      if (TrackedController != null)
        return Math.Min(Game1.ScreenWidth, Game1.ScreenHeight) / (900 + 1 * TrackedController.Radius);
      else
        return 1;
    }
  }

  public float BuildMenuZoom
  {
    get
    {
      if (TrackedController != null)
        return Math.Min(Game1.ScreenWidth, Game1.ScreenHeight) / (2 * TrackedController.Radius + 900 / 8);
      else
        return 1;
    }
  }

  private float maxZoom = 6;
  private float minZoom = 0.5f;

  public Camera(IModuleContainer trackedController = null)
  {
    TrackedController = trackedController;
    Position = trackedController?.Position ?? Vector2.Zero;
    PreviousPosition = Position;
    Rotation = 0;
    Zoom = 1;
    AutoAdjustZoom = true;
    UpdateTransformMatrix();
  }

  public void Update(GameTime gameTime)
  {
    PreviousPosition = Position;

    // Only update position if not locked and we have a controller to track
    if (!IsLocked && TrackedController != null)
    {
      Position = TrackedController.Position;
    }

    // Handle auto zoom adjustment
    if (AutoAdjustZoom)
    {
      float targetZoom = InBuildScreen ? BuildMenuZoom : GameZoom;
      AdjustZoom(targetZoom);
    }

    UpdateTransformMatrix();
  }

  private void AdjustZoom(float optimalZoom)
  {
    if (IsLocked) return;

    float zoomSpeed = InBuildScreen ? 10f : 0.01f;

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

  public Vector2 WorldToScreen(Vector2 worldPosition)
  {
    float x = (worldPosition.X - Position.X) * Zoom + (Game1.ScreenWidth / 2);
    float y = (worldPosition.Y - Position.Y) * Zoom + (Game1.ScreenHeight / 2);
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

  public void LockToPosition(Vector2 lockPosition)
  {
    IsLocked = true;
    Position = lockPosition;
    UpdateTransformMatrix();
  }

  public void Unlock()
  {
    IsLocked = false;
  }
}
