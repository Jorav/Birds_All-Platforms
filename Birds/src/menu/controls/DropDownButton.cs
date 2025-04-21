using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.menu.controls
{
    public class DropDownButton //: Button
    {/*
        List<(Button, IDs)> buttons;
        bool isClicked;
        public (Button, IDs) currentSelection;
        public override Vector2 Position { get { return position; } 
            set 
            {
                Vector2 posChange = value-position;
                foreach ((Button, IDs) t in buttons)
                    t.Item1.Position += posChange;
                base.Position = value;
            } 
        }
        public DropDownButton(Sprite sprite, SpriteFont font = null, IDs[] ids = null) : base(sprite, font)
        {
            buttons = new List<(Button, IDs)>();
            if (ids != null)
            {
                foreach (IDs id in ids)
                {
                    Button b = new Button((Sprite)sprite.Clone(), font);
                    b.Text = ControllerFactory.GetName(id);
                    b.Position = Position + (buttons.Count+1) * new Vector2(0, sprite.Height);
                    b.Click += SubButton_Click;
                    buttons.Add((b, id));
                }
                Text = ControllerFactory.GetName(ids[0]);
            }
        }

        private void SubButton_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            currentSelection = buttons.Find(t => t.Item1 == b);
            Text = ControllerFactory.GetName(currentSelection.Item2);
            isClicked = false;
            InvokeEvent(new EventArgs());
        }

        protected override void HandleMouse()
        {
            if (MouseIntersects())
            {
                isHovering = true;
                if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                {
                    isClicked = !isClicked;
                }
            }
            else if(currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
            {
                bool clickedOutside = true;
                foreach ((Button, IDs) t in buttons)
                    if (t.Item1.MouseIntersects())
                        clickedOutside = false;
                if (clickedOutside)
                    isClicked = false;
            }
            
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isClicked)
            {
                isHovering = true;
                foreach ((Button, IDs) t in buttons)
                    t.Item1.Update(gameTime);
            }
        }
        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
            if (isClicked)
                foreach ((Button, IDs) t in buttons)
                    t.Item1.Draw(spritebatch);
        }*/
    }
}
