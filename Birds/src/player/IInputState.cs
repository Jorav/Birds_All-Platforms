using Microsoft.Xna.Framework;

namespace Birds.src.player;

public interface IInputState
{
  bool IsPressed { get; }
  bool WasPressed { get; }
  bool WasJustPressed { get; }
  bool WasJustReleased { get; }
  Vector2 PositionGameCoords { get; }
  Vector2 CameraPosition { get; }
  float CameraZoom { get; }
}
