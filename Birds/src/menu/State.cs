using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.menu
{
    public abstract class State
    {
        #region Fields
        protected ContentManager content;
        protected GraphicsDevice graphicsDevice;
        protected Game1 game;
        protected Input input;
        #endregion

        #region Methods
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void PostUpdate();

        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input)
        {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            this.content = content;
            this.input = input;
        }

        public abstract void Update(GameTime gameTime);
        #endregion
    }
}
