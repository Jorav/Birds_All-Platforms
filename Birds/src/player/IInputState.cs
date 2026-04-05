using Microsoft.Xna.Framework;

namespace Birds.src.player;

public interface IInputState
{
  bool IsPressed { get; }
  Vector2 PositionGameCoords { get; }
  Vector2 CameraPosition { get; }
  float CameraZoom { get; }
}