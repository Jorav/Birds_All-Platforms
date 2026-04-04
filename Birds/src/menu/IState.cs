using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.menu;

public interface IState
{
  bool IsLocked { get; set; }
  void Update(GameTime gameTime);
  void PostUpdate();
  void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
