using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.player;

public class Camera(IModuleContainer trackedController = null)
{
  public Matrix Transform { get; private set; }
  public Vector2 Position { get; set; } = trackedController?.Position ?? Vector2.Zero;
  public Vector2 PreviousPosition { get; set; } = trackedController?.Position ?? Vector2.Zero;
  public bool IsLocked { get; set; }
  public float Rotation { get; set; } = 0;

  private float zoom = 1;
  public float Zoom
  {
    get => zoom;
    set
    {
      if (value > maxZoom) zoom = maxZoom;
      else if (value < minZoom) zoom = minZoom;
      else zoom = value;
    }
  }

  private bool inBuildScreen;
  public bool InBuildScreen
  {
    get => inBuildScreen;
    set
    {
      Zoom = value ? BuildMenuZoom : GameZoom;
      inBuildScreen = value;
      UpdateTransformMatrix();
    }
  }

  public float Width => Game1.ScreenWidth / Zoom;
  public float Height => Game1.ScreenHeight / Zoom;
  public bool AutoAdjustZoom { get; set; } = true;

  private IModuleContainer _trackedController = trackedController;
  public IModuleContainer TrackedController
  {
    get => _trackedController;
    set
    {
      if (value != null)
      {
        Position = value.Position;
        PreviousPosition = value.Position;
      }
      _trackedController = value;
    }
  }

  public float GameZoom => TrackedController != null
      ? Math.Min(Game1.ScreenWidth, Game1.ScreenHeight) / (900 + 1 * TrackedController.Radius)
      : 1;

  public float BuildMenuZoom => TrackedController != null
      ? Math.Min(Game1.ScreenWidth, Game1.ScreenHeight) / (2 * TrackedController.Radius + 900 / 8)
      : 1;

  private float maxZoom = 6;
  private float minZoom = 0.5f;

  public void Update(GameTime gameTime)
  {
    PreviousPosition = Position;

    if (!IsLocked && TrackedController != null)
    {
      Position = TrackedController.Position;
    }

    if (AutoAdjustZoom)
    {
      AdjustZoom(InBuildScreen ? BuildMenuZoom : GameZoom);
    }

    UpdateTransformMatrix();
  }

  private void AdjustZoom(float optimalZoom)
  {
    if (IsLocked) return;
    float zoomSpeed = InBuildScreen ? 10f : 0.01f;

    if (optimalZoom > Zoom)
      Zoom = (optimalZoom / Zoom > 1 + zoomSpeed) ? Zoom * (1 + zoomSpeed) : optimalZoom;
    else if (optimalZoom < Zoom)
      Zoom = (Zoom / optimalZoom > 1 + zoomSpeed) ? Zoom / (1 + zoomSpeed) : optimalZoom;
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
    Matrix scale = Matrix.CreateScale(Zoom, Zoom, 0);
    Transform = position * rotation * scale * origin;
  }

  public void LockToPosition(Vector2 lockPosition)
  {
    IsLocked = true;
    Position = lockPosition;
    UpdateTransformMatrix();
  }

  public Rectangle GetVisibleBounds(float zoom, float buffer = 0f)
  {
    float width = Game1.ScreenWidth / zoom;
    float height = Game1.ScreenHeight / zoom;
    float halfWidth = width / 2f + buffer;
    float halfHeight = height / 2f + buffer;

    return new Rectangle(
      (int)(Position.X - halfWidth),
      (int)(Position.Y - halfHeight),
      (int)(width + buffer * 2),
      (int)(height + buffer * 2)
    );
  }

  public bool IsEntityWithinFrame(ModuleContainer entity, float zoom, float buffer = 0f)
  {
    var bounds = GetVisibleBounds(zoom, buffer);
    var pos = entity.Position.Value;
    var radius = entity.Radius.Value;

    return bounds.Contains((int)pos.X, (int)pos.Y) ||
           bounds.Intersects(new Rectangle(
             (int)(pos.X - radius),
             (int)(pos.Y - radius),
             (int)(radius * 2),
             (int)(radius * 2)
           ));
  }

  public Matrix GetParallaxTransform(float parallaxFactor)
  {
    Matrix position = Matrix.CreateTranslation(-Position.X * parallaxFactor, -Position.Y * parallaxFactor, 0);
    Matrix scale = Matrix.CreateScale(Zoom, Zoom, 0);
    Matrix origin = Matrix.CreateTranslation(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2, 0);
    return position * scale * origin;
  }

  public void Unlock() => IsLocked = false;
}
