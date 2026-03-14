using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.menu;

public interface IComponent
{
  public void Draw(SpriteBatch spritebatch);

  public void Update(GameTime gameTime);
}

