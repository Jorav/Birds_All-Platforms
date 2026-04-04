using Microsoft.Xna.Framework;

namespace Birds.src.player;

public interface IPointerDevice
{
  Vector2 ScreenPosition { get; }
  Vector2 PreviousScreenPosition { get; }
  bool IsPressed { get; }
  bool WasPressed { get; }
  bool IsReleased { get; }
  bool WasJustPressed => IsPressed && !WasPressed;
  bool WasJustReleased => !IsPressed && WasPressed;
  void Update(GameTime gameTime);
  void HandleZoom(Camera camera);
}
