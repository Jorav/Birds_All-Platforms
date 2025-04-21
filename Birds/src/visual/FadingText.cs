using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.visual
{
    public class FadingText
    {
        public String Text { get; set; }
        protected SpriteFont font;
        public Vector2 Position { get; set; }
        public Color originalColor;
        protected Color currentColor;
        private float timer = 0;
        public float MaxTime { get; set; }
        public bool IsVisible { get; set; }
        public Vector2 Measurements { get {  return font.MeasureString(Text); } }

        public FadingText(String Text, Vector2 Position, SpriteFont font = null, float maxTime = 0)
        {
            this.Text = Text;
            this.Position = Position;
            this.font = font;
            originalColor = Color.Black;
            currentColor = originalColor;
            MaxTime = maxTime;
            IsVisible = false;
        }

        public void Update(GameTime gameTime)
        {
            if (MaxTime > 0 && IsVisible)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timer > MaxTime)
                    IsVisible = false;
                currentColor = originalColor * (1-(timer / MaxTime));
            }
        }

        public void Display()
        {
            IsVisible = true;
            currentColor = originalColor;
            timer = 0;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            //float x = (Rectangle.X + (Rectangle.Width / 2)) - font.MeasureString(Text).X / 2;
            //float y = (Rectangle.Y + (Rectangle.Height / 2)) - font.MeasureString(Text).Y / 2;
            if (IsVisible && !string.IsNullOrEmpty(Text) && font!=null)
            {
                float x = Position.X - font.MeasureString(Text).X / 2;
                float y = Position.Y - font.MeasureString(Text).Y / 2;
                spritebatch.DrawString(font, Text, new Vector2(x, y), currentColor);
            }
        }
    }
}
