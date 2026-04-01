using Birds.src.events;
using Birds.src.utility;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller;

public class CameraModule : ModuleBase
{
  public Vector2 Position { get; set; }
  private Camera camera;

  public CameraModule()
  {
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);

    // Get the global camera from Input
    camera = Input.Camera;

    if (camera != null)
    {
      camera.Controller = container;
    }
  }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    if (camera == null) return;

    // Update camera position to match controller's center of mass
    // This happens AFTER GroupWeightedPositionModule runs, so position is already correct
    camera.PreviousPosition = camera.Position;

    if (!camera.IsLocked)
    {
      camera.Position = Position;
    }

    // Handle zoom
    if (camera.AutoAdjustZoom)
    {
      float targetZoom = camera.InBuildScreen ? camera.BuildMenuZoom : camera.GameZoom;
      AdjustZoom(targetZoom);
    }

    camera.Rotation = 0;
    camera.UpdateTransformMatrix();
  }

  private void AdjustZoom(float optimalZoom)
  {
    if (camera.IsLocked) return;

    float zoomSpeed = camera.InBuildScreen ? 10f : 0.01f;

    if (optimalZoom > camera.Zoom)
    {
      if (optimalZoom / camera.Zoom > 1 + zoomSpeed)
        camera.Zoom *= 1 + zoomSpeed;
      else
        camera.Zoom = optimalZoom;
    }
    else if (optimalZoom < camera.Zoom)
    {
      if (camera.Zoom / optimalZoom > 1 + zoomSpeed)
        camera.Zoom /= 1 + zoomSpeed;
      else
        camera.Zoom = optimalZoom;
    }
  }

  public override object Clone()
  {
    var cloned = (CameraModule)base.Clone();
    cloned.camera = null;
    return cloned;
  }
}