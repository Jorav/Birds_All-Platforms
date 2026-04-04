using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Birds.src.player;

public class MouseDevice : IPointerDevice
{
  private float previousScrollValue;
  private Vector2 previousScreenPosition;

  public Vector2 ScreenPosition { get; private set; }
  public Vector2 PreviousScreenPosition => previousScreenPosition;
  public bool IsPressed { get; private set; }
  public bool WasPressed { get; private set; }
  public bool IsReleased { get; private set; }

  public void Update(GameTime gameTime)
  {
    WasPressed = IsPressed;
    previousScreenPosition = ScreenPosition;
    var mouse = Mouse.GetState();
    ScreenPosition = mouse.Position.ToVector2();
    IsPressed = mouse.LeftButton == ButtonState.Pressed;
    IsReleased = mouse.LeftButton == ButtonState.Released;
  }

  public void HandleZoom(Camera camera)
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
