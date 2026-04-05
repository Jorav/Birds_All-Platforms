using Birds.src.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Birds.src.player;

public class Input(
  InputConfiguration config,
  IPointerDevice pointer) : IInputState
{
  private bool pauseDown;
  private bool buildDown;
  private bool enterDown;

  public Camera Camera { get; set; }

  public Vector2 ScreenPosition => pointer.ScreenPosition;
  public Vector2 PreviousScreenPosition => pointer.PreviousScreenPosition;
  public bool IsPressed => pointer.IsPressed;
  public bool WasPressed => pointer.WasPressed;
  public bool IsReleased => pointer.IsReleased;
  public bool WasJustPressed => pointer.WasJustPressed;
  public bool WasJustReleased => pointer.WasJustReleased;

  public Vector2 PositionGameCoords => Camera?.ScreenToWorld(ScreenPosition) ?? ScreenPosition;

  public void Update(GameTime gameTime)
  {
    pointer.Update(gameTime);
  }
  public void HandleZoom() => pointer.HandleZoom(Camera);

  public bool PauseClicked => CheckOnce(ref pauseDown, config.Pause);
  public bool BuildClicked => CheckOnce(ref buildDown, config.Build);
  public bool EnterClicked => CheckOnce(ref enterDown, config.Enter);

  public Vector2 CameraPosition => Camera.Position;

  public float CameraZoom => Camera.Zoom;

  private bool CheckOnce(ref bool wasDown, Keys key)
  {
    bool isDown = Keyboard.GetState().IsKeyDown(key);
    bool clicked = !wasDown && isDown;
    wasDown = isDown;
    return clicked;
  }
}
