using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.menu
{
    public interface IComponent
    {
        public void Draw(SpriteBatch spritebatch);

        public void Update(GameTime gameTime);
    }
}
