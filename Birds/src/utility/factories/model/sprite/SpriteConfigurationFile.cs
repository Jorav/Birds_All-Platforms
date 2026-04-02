using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.utility.factories.model.sprite;

public class SpriteConfigurationFile
{
  public SpriteConfiguration Default { get; set; }
  public Dictionary<string, SpriteConfiguration> Sprites { get; set; }
}

public class SpriteConfiguration
{
  public float? Alpha { get; set; }
  public ColorConfig Color { get; set; }
  public bool Visible { get; set; } = true;
}

public class ColorConfig
{
  public byte R { get; set; } = 255;
  public byte G { get; set; } = 255;
  public byte B { get; set; } = 255;
  public byte A { get; set; } = 255;

  public Color ToColor()
  {
    return new Color(R, G, B, A);
  }
}