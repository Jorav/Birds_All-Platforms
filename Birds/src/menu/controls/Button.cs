using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Birds.src.visual;
using System;
using System.Collections.Generic;
using System.Text;
using Birds.src.utility;

namespace Birds.src.menu.controls
{
    public class Button : IComponent
    {
        #region Fields
        //protected SpriteFont font;
        protected internal bool isHovering;
        protected Sprite sprite;
        #endregion

        #region Properties
        public event EventHandler Click;
        public bool Clicked { get; private set; }
        //public Color PenColour;
        protected Vector2 position;
        public virtual Vector2 Position { get { return position; } set { sprite.Position = value; TextNew.Position = value+new Vector2(Rectangle.Width/2, Rectangle.Height/2); position = value; } }
        protected float scale;
        public virtual float Scale { get { return scale; } set { sprite.Scale = value; scale = value; } } //doesnt work with text
        public Vector2 Dimensions { get { return new Vector2(sprite.Width, sprite.Height); } }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
            }
        }
        public String Text { get { return TextNew.Text; } set { TextNew.Text = value; } }
        public FadingText TextNew { get; set; }
        #endregion

        #region Methods
        public Button(Sprite sprite, SpriteFont font = null, String text = null) 
        {
            this.sprite = sprite;
            position = sprite.Position;
            //this.font = font;
            //PenColour = Color.Black;
            sprite.Origin = Vector2.Zero;
            TextNew = new FadingText(text, Position, font);
            TextNew.IsVisible = true;
            //Scale = 1;
        }
        public bool IsHovering()
        {
            Vector2 position = Input.Position;
            Rectangle mouseRectangle = new Rectangle(((int)Math.Round(position.X)), ((int)Math.Round(position.Y)), 1, 1);
            return mouseRectangle.Intersects(Rectangle);
        }
        public virtual void Update(GameTime gameTime)
        {
            isHovering = false;
            HandleInput();
        }

        protected virtual void HandleInput()
        {
            if (IsHovering())
            {
                isHovering = true;
                //if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                if (Input.IsPressed)
                {
                    InvokeEvent(new EventArgs());
                }
            }
        }
        protected void InvokeEvent(EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            sprite.Color = Color.White;
            if (isHovering)
                sprite.Color = Color.Gray;
            //spritebatch.Draw(texture, Rectangle, color)
            sprite.Draw(spritebatch);

            if (!string.IsNullOrEmpty(Text))
            {
                TextNew.Draw(spritebatch);
                //float x = (Rectangle.X + (Rectangle.Width / 2)) - font.MeasureString(Text).X / 2;
                //float y = (Rectangle.Y + (Rectangle.Height / 2)) - font.MeasureString(Text).Y / 2;
                //spritebatch.DrawString(font, Text, new Vector2(x, y), PenColour);
            }

        }
        #endregion
    }
}
