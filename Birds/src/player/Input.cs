using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace Birds.src.player;

public class Input
{
  public Keys Up { get; set; }
  public Keys Down { get; set; }
  public Keys Left { get; set; }
  public Keys Right { get; set; }
  public Keys Pause { get; set; }
  public Keys Build { get; set; }
  public Keys Enter { get; set; }

  private int trackedTLID = -1;
  private float previousScrollValue;
  private bool pinching = false;
  private float pinchPreviousDistance;
  private Vector2 previousPosition = Vector2.Zero;

  private bool pauseDown;
  private bool buildDown;
  private bool enterDown;

  private Camera camera;

  public Input(Camera camera)
  {
    this.camera = camera;
    Position = Vector2.Zero;
    previousPosition = Vector2.Zero;
  }

  public Vector2 Position { get; set; }
  public Vector2 PreviousPosition => previousPosition;
  public bool IsPressed { get; set; }
  public bool WasPressed { get; set; }
  public bool IsReleased { get; set; }

  public bool WasJustPressed => IsPressed && !WasPressed;
  public bool WasJustReleased => !IsPressed && WasPressed;

  public Vector2 PositionGameCoords => camera.ScreenToWorld(Position);

  public void Update(GameTime gameTime)
  {
    WasPressed = IsPressed;

    UpdatePosition();
    UpdateIsPressed();
    UpdateIsReleased();
  }

  private void UpdateIsReleased()
  {
    TouchPanelCapabilities tc = TouchPanel.GetCapabilities();
    if (tc.IsConnected)
    {
      TouchCollection touchCollection = TouchPanel.GetState();
      if (trackedTLID == -1)
      {
        bool anyPressed = false;
        foreach (TouchLocation tl in touchCollection)
        {
          if (tl.State == TouchLocationState.Pressed)
          {
            anyPressed = true;
          }
        }
        IsReleased = !anyPressed;
      }
      else
      {
        IsReleased = false;
        foreach (TouchLocation tl in touchCollection)
        {
          if (tl.Id == trackedTLID)
          {
            if (tl.State == TouchLocationState.Released)
            {
              trackedTLID = -1;
              IsReleased = true;
            }
          }
        }
      }
    }
    else
    {
      IsReleased = Mouse.GetState().LeftButton == ButtonState.Released;
    }
  }

  private void UpdateIsPressed()
  {
    TouchPanelCapabilities tc = TouchPanel.GetCapabilities();
    if (tc.IsConnected && !pinching)
    {
      IsPressed = false;
      TouchCollection touchCollection = TouchPanel.GetState();
      foreach (TouchLocation tl in touchCollection)
      {
        if ((tl.State == TouchLocationState.Pressed) || (tl.State == TouchLocationState.Moved))
        {
          IsPressed = true;
        }
      }
    }
    else
    {
      IsPressed = Mouse.GetState().LeftButton == ButtonState.Pressed;
    }
  }

  private void UpdatePosition()
  {
    TouchPanelCapabilities tc = TouchPanel.GetCapabilities();
    if (tc.IsConnected)
    {
      TouchCollection touchCollection = TouchPanel.GetState();
      if (trackedTLID != -1)
      {
        foreach (TouchLocation tl in touchCollection)
        {
          if (tl.Id == trackedTLID)
          {
            if (tl.State == TouchLocationState.Released)
              trackedTLID = -1;
          }
        }
      }
      if (trackedTLID == -1)
      {
        foreach (TouchLocation tl in touchCollection)
        {
          if ((tl.State == TouchLocationState.Pressed) || (tl.State == TouchLocationState.Moved))
          {
            trackedTLID = tl.Id;
          }
        }
      }
      if (!pinching && trackedTLID != -1)
      {
        foreach (TouchLocation tl in touchCollection)
        {
          if (tl.Id == trackedTLID && ((tl.State == TouchLocationState.Pressed) || (tl.State == TouchLocationState.Moved)))
          {
            previousPosition = Position;
            Position = tl.Position;
          }
        }
      }
    }
    else
    {
      previousPosition = Position;
      Position = Mouse.GetState().Position.ToVector2();
    }
  }

  public void HandleZoom()
  {
    TouchPanelCapabilities tc = TouchPanel.GetCapabilities();
    if (tc.IsConnected)
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
          if (l1 == null)
            l1 = tl.Position;
          else
            l2 = tl.Position;
        }
      }
      if (pressedLocations == 2)
      {
        float distance = Vector2.Distance((Vector2)l1, (Vector2)l2);
        if (!pinching)
        {
          pinchPreviousDistance = distance;
        }
        float scale = distance / pinchPreviousDistance;
        camera.Zoom *= scale;
        camera.AutoAdjustZoom = false;
        pinching = true;
        pinchPreviousDistance = distance;
      }
      else
        pinching = false;
    }
    else
    {
      float scrollValue = Mouse.GetState().ScrollWheelValue;
      if (previousScrollValue - scrollValue != 0)
      {
        camera.Zoom /= (float)Math.Pow(0.999, (scrollValue - previousScrollValue));
        camera.AutoAdjustZoom = false;
      }
      previousScrollValue = scrollValue;
    }
  }

  public bool PauseClicked
  {
    get
    {
      bool pauseClicked = false;
      bool newPauseDown = Keyboard.GetState().IsKeyDown(Pause);
      if (!pauseDown && newPauseDown)
      {
        pauseClicked = true;
      }
      pauseDown = newPauseDown;
      return pauseClicked;
    }
  }

  public bool BuildClicked
  {
    get
    {
      bool buildClicked = false;
      bool newBuildDown = Keyboard.GetState().IsKeyDown(Build);
      if (!buildDown && newBuildDown)
      {
        buildClicked = true;
      }
      buildDown = newBuildDown;
      return buildClicked;
    }
  }

  public bool EnterClicked
  {
    get
    {
      bool enterClicked = false;
      bool newEnterDown = Keyboard.GetState().IsKeyDown(Enter);
      if (!enterDown && newEnterDown)
      {
        enterClicked = true;
      }
      enterDown = newEnterDown;
      return enterClicked;
    }
  }
}
