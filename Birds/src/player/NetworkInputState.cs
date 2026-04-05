using Birds.src.api.contracts;
using Microsoft.Xna.Framework;

namespace Birds.src.player;

public class NetworkInputState : IInputState
{
  public bool IsPressed { get; set; }
  public Vector2 PositionGameCoords { get; set; }
  public Vector2 CameraPosition { get; set; }
  public float CameraZoom { get; set; } = 1f;

  public void ApplyInput(InputMessage msg)
  {
    IsPressed = msg.IsPressed;
    PositionGameCoords = msg.PositionGameCoords;
    CameraPosition = msg.CameraPosition;
    CameraZoom = msg.CameraZoom;
  }
}
