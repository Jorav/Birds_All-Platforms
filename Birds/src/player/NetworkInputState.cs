using Birds.src.api.contracts;
using Microsoft.Xna.Framework;

namespace Birds.src.player;

public class NetworkInputState : IInputState
{
  public bool IsPressed { get; private set; }
  public bool WasPressed { get; private set; }
  public bool WasJustPressed { get; private set; }
  public bool WasJustReleased { get; private set; }
  public Vector2 PositionGameCoords { get; private set; }
  public Vector2 CameraPosition { get; private set; }
  public float CameraZoom { get; private set; } = 1f;

  public void ApplyInput(InputMessage msg)
  {
    WasPressed = IsPressed;
    IsPressed = msg.IsPressed;
    WasJustPressed = !WasPressed && IsPressed;
    WasJustReleased = WasPressed && !IsPressed;
    PositionGameCoords = msg.PositionGameCoords;
    CameraPosition = msg.CameraPosition;
    CameraZoom = msg.CameraZoom;
  }
}
