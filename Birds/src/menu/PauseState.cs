using Birds.src.menu.controls;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.menu
{/*
    class PauseState : MenuState
    {
        State previousState;
        public PauseState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, State previousState, Input input) : base(game, graphicsDevice, content, input)
        {
            this.previousState = previousState;
            Texture2D buttonTexture = content.Load<Texture2D>("button");
            SpriteFont buttonFont = content.Load<SpriteFont>("font");
            Sprite background = new Sprite(content.Load<Texture2D>("button")); //backgroundGray
            background.Scale = background.Height/ Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);

            Button continueButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(300, 150), //or preferably center
                Text = "Continue",
            };
            continueButton.Click += ContinueButton_Click;

            Button newGameButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(300, 200), //or preferably center
                Text = "New Game",
            };
            newGameButton.Click += NewGameButton_Click;

            Button loadGameButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(300, 250), //or preferably center
                Text = "Load Game",
            };
            loadGameButton.Click += LoadGameButton_Click;

            Button mainMenuButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(300, 300), //or preferably center
                Text = "Main Menu",
            };
            mainMenuButton.Click += MainMenuButton_Click;

            Button quitGameButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(300, 350), //or preferably center
                Text = "Quit Game",
            };
            quitGameButton.Click += QuitGameButton_Click;

            components = new List<IComponent>()
            {
                background,
                continueButton,
                newGameButton,
                loadGameButton,
                mainMenuButton,
                quitGameButton,
            };
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            previousState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (input.PauseClicked)
                game.ChangeState(previousState);
        }
        private void ContinueButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(previousState);
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new TestState(game, graphicsDevice, content, input));
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            //load game state from earlier
            throw new NotImplementedException();
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new MainMenu(game, graphicsDevice, content, input));
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }
    }*/
}
