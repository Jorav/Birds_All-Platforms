using Microsoft.Xna.Framework;

namespace Birds.src.api.contracts;

public class InputMessage
{
  public string PlayerId { get; set; }
  public long Tick { get; set; }
  public bool IsPressed { get; set; }
  public Vector2 PositionGameCoords { get; set; }
  public Vector2 CameraPosition { get; set; }
  public float CameraZoom { get; set; }
}
