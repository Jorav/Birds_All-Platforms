using Microsoft.Xna.Framework.Input;

namespace Birds.src.player;

public class InputConfiguration
{
  public Keys Up { get; set; } = Keys.W;
  public Keys Down { get; set; } = Keys.S;
  public Keys Left { get; set; } = Keys.A;
  public Keys Right { get; set; } = Keys.D;
  public Keys Pause { get; set; } = Keys.Escape;
  public Keys Build { get; set; } = Keys.B;
  public Keys Enter { get; set; } = Keys.Enter;

  public static InputConfiguration LoadDefault()
  {
    // TODO: Load from file/settings
    return new InputConfiguration();
  }
}
