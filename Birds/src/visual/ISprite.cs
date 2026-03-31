using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.menu;

namespace Birds.src.visual;

public interface ISprite : IComponent
{
  Vector2 Position { get; set; }
  Vector2 Origin { get; set; }
  int Height { get; }
  int Width { get; }
  Color Color { get; set; }
  float Alpha { get; set; }
  float Scale { get; set; }
}
