using Birds.src.factories;
using Birds.src.utility;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System;

namespace Birds.src.menu.controls
{
    public class ButtonContainer : IComponent
    {
        List<Button> buttons;
        private ID_POSITION desiredPosition;
        private Vector2 position;
        private float bufferDistance = 10f;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                Vector2 posChange = value - position;
                foreach (Button t in buttons)
                    t.Position += posChange;
                position = value;
            }
        }
        public ButtonContainer(ID_POSITION desiredPosition, List<Button> buttons)
        {
            this.buttons = buttons;
            this.desiredPosition = desiredPosition;
            SetButtonsToPosition(desiredPosition);
        }

        private void SetButtonsToPosition(ID_POSITION position)
        {
            this.desiredPosition = position;
            float totalHeight = 0;
            foreach(Button b in buttons)
            {
                totalHeight += b.Dimensions.Y;
                totalHeight += bufferDistance;
            }
            totalHeight -= bufferDistance;
            float currentY = 0f;
            switch (position)
            {
                case ID_POSITION.POSITION_MIDDLE:
                    currentY = Game1.ScreenHeight / 2 - totalHeight / 2;
                    foreach (Button b in buttons)
                    {
                        b.Position = new Vector2(Game1.ScreenWidth / 2 - b.Dimensions.X / 2, currentY);
                        currentY += b.Dimensions.Y + bufferDistance;
                    }
                    break;
                case ID_POSITION.POSITION_TOP_RIGHT:
                    currentY = bufferDistance;
                    foreach (Button b in buttons)
                    {
                        b.Position = new Vector2(Game1.ScreenWidth - b.Dimensions.X-bufferDistance, currentY);
                        currentY += b.Dimensions.Y + bufferDistance;
                    }
                    break;
                throw new NotImplementedException();
            }
        }
        public void Update(GameTime gameTime)
        {
            SetButtonsToPosition(desiredPosition);
            foreach (Button b in buttons)
                b.Update(gameTime);
        }
        public void Draw(SpriteBatch spritebatch)
        {
            foreach (Button b in buttons)
                b.Draw(spritebatch);
        }
    }
}
