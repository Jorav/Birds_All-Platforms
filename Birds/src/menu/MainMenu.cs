using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.menu.controls;
using System;
using System.Collections.Generic;
using System.Text;
using Birds.src.utility;
using Birds.src.factories;

namespace Birds.src.menu
{
    public class MainMenu : MenuState
    {
        public MainMenu(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input) : base(game, graphicsDevice, content, input)
        {
            this.input = input;

            Button newGameButton = new Button(SpriteFactory.GetSprite(ID_SPRITE.BUTTON, new Vector2(Game1.ScreenWidth/2-100, Game1.ScreenHeight/2-100)), Game1.font)
            {
                Text = "Start",
            };
            newGameButton.Click += NewGameButton_Click;

            /*Button buildModeButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(300, 250), //or preferably center
                Text = "Build Mode",
            };
            buildModeButton.Click += BuildModeButton_Click;*/

            /*Button loadGameButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(300, 300), //or preferably center
                Text = "Load Game",
            };
            loadGameButton.Click += LoadGameButton_Click;*/

            Button quitButton = new Button(SpriteFactory.GetSprite(ID_SPRITE.BUTTON, new Vector2(Game1.ScreenWidth/2-100, Game1.ScreenHeight/2-100+50)), Game1.font)
            {
                Text = "Quit",
            };
            quitButton.Click += QuitButton_Click;
            Sprite background = SpriteFactory.GetSprite(ID_SPRITE.BACKGROUND_GRAY, new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2), SpriteFactory.textures[(int)ID_SPRITE.BACKGROUND_WHITE].Height / Game1.ScreenHeight);
            ButtonContainer container = new ButtonContainer(ID_POSITION.POSITION_MIDDLE, new List<Button>  { newGameButton });
            components = new List<IComponent>()
            {
                background,
                container,
                //newGameButton,
                //buildModeButton,
                //loadGameButton,
                //quitButton,
            };
        }


        private void NewGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new TestState(game, graphicsDevice, content, input));
        }

        /*private void BuildModeButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new WorldEditor(game, graphicsDevice, content, input));
        }*/

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            //load game state from earlier
            throw new NotImplementedException();
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }
        
    }
}
