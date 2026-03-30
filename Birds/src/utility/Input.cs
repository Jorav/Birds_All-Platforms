using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace Birds.src.utility;

public class Input
{
  public static Camera Camera { get; set; }
  public Keys Up { get; set; }
  public Keys Down { get; set; }
  public Keys Left { get; set; }
  public Keys Right { get; set; }
  public Keys Pause { get; set; }
  public Keys Build { get; set; }
  public Keys Enter { get; set; }

  private static int trackedTLID = -1;
  private static float previousScrollValue;
  private static bool pinching = false;
  private static float pinchPreviousDistance;
  private static Vector2 previousPosition = Vector2.Zero;
  private static bool previousIsPressed = false;

  private bool pauseDown;
  private bool buildDown;
  private bool enterDown;

  public static void Update(GameTime gameTime)
  {
    previousIsPressed = IsPressed;

    UpdatePosition();
    UpdateIsPressed();
    UpdateIsReleased();
    if (Camera != null)
      Camera.Update();
  }

  private static void UpdateIsReleased()
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
      IsReleased = false;
    }
    else
    {
      IsReleased = Mouse.GetState().LeftButton == ButtonState.Released;
    }
  }

  private static void UpdateIsPressed()
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

  private static void UpdatePosition()
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

  public static void HandleZoom()
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
        Camera.Zoom *= scale;
        Camera.AutoAdjustZoom = false;
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
        Camera.Zoom /= (float)Math.Pow(0.999, (scrollValue - previousScrollValue));
        Camera.AutoAdjustZoom = false;
      }
      previousScrollValue = scrollValue;
    }
  }

  public static Vector2 PositionGameCoords { get { return Camera.ScreenToWorld(Position); } }

  public static Vector2 Position
  {
    set; get;
  } = previousPosition;

  public static bool IsPressed
  {
    get; set;
  }

  public static bool WasPressed
  {
    get { return IsPressed && !previousIsPressed; }
  }

  public static bool IsReleased
  {
    get; set;
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