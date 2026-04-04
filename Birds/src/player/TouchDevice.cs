using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Birds.src.player;

public class TouchDevice : IPointerDevice
{
  private int trackedTLID = -1;
  private bool pinching = false;
  private float pinchPreviousDistance;
  private Vector2 previousScreenPosition;

  public Vector2 ScreenPosition { get; private set; }
  public Vector2 PreviousScreenPosition => previousScreenPosition;
  public bool IsPressed { get; private set; }
  public bool WasPressed { get; private set; }
  public bool IsReleased { get; private set; }
  public bool WasJustPressed => IsPressed && !WasPressed;
  public bool WasJustReleased => !IsPressed && WasPressed;

  public void Update(GameTime gameTime)
  {
    WasPressed = IsPressed;
    UpdateTracking();
    UpdateIsPressed();
    UpdateIsReleased();
  }

  private void UpdateTracking()
  {
    TouchCollection touchCollection = TouchPanel.GetState();

    if (trackedTLID != -1)
    {
      foreach (TouchLocation tl in touchCollection)
      {
        if (tl.Id == trackedTLID && tl.State == TouchLocationState.Released)
          trackedTLID = -1;
      }
    }

    if (trackedTLID == -1)
    {
      foreach (TouchLocation tl in touchCollection)
      {
        if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
          trackedTLID = tl.Id;
      }
    }

    if (!pinching && trackedTLID != -1)
    {
      foreach (TouchLocation tl in touchCollection)
      {
        if (tl.Id == trackedTLID && (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved))
        {
          previousScreenPosition = ScreenPosition;
          ScreenPosition = tl.Position;
        }
      }
    }
  }

  private void UpdateIsPressed()
  {
    if (pinching)
    {
      IsPressed = false;
      return;
    }

    TouchCollection touchCollection = TouchPanel.GetState();
    IsPressed = false;
    foreach (TouchLocation tl in touchCollection)
    {
      if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
        IsPressed = true;
    }
  }

  private void UpdateIsReleased()
  {
    TouchCollection touchCollection = TouchPanel.GetState();

    if (trackedTLID == -1)
    {
      bool anyPressed = false;
      foreach (TouchLocation tl in touchCollection)
      {
        if (tl.State == TouchLocationState.Pressed)
          anyPressed = true;
      }
      IsReleased = !anyPressed;
    }
    else
    {
      IsReleased = false;
      foreach (TouchLocation tl in touchCollection)
      {
        if (tl.Id == trackedTLID && tl.State == TouchLocationState.Released)
        {
          trackedTLID = -1;
          IsReleased = true;
        }
      }
    }
  }

  public void HandleZoom(Camera camera)
  {
    TouchCollection touchCollection = TouchPanel.GetState();
    int pressedLocations = 0;
    Vector2? l1 = null;
    Vector2? l2 = null;

    foreach (TouchLocation tl in touchCollection)
    {
      if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
      {
        pressedLocations++;
        if (l1 == null) l1 = tl.Position;
        else l2 = tl.Position;
      }
    }

    if (pressedLocations == 2)
    {
      float distance = Vector2.Distance((Vector2)l1, (Vector2)l2);
      if (!pinching) pinchPreviousDistance = distance;
      float scale = distance / pinchPreviousDistance;
      camera.Zoom *= scale;
      camera.AutoAdjustZoom = false;
      pinching = true;
      pinchPreviousDistance = distance;
    }
    else
    {
      pinching = false;
    }
  }
}
