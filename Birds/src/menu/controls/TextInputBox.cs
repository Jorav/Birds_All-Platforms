using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Birds.src.menu.controls;

public class TextInputBox : IComponent
{
  public string Text { get; private set; } = "";
  public Rectangle Rectangle { get; set; }
  public bool IsActive { get; set; }
  private SpriteFont font;
  private Texture2D background;
  private bool _showCursor;
  private double _cursorTimer;

  private KeyboardState _prevKeyState;

  public TextInputBox(Rectangle bounds, SpriteFont font, GraphicsDevice graphicsDevice)
  {
    Rectangle = bounds;
    this.font = font;
    background = new Texture2D(graphicsDevice, 1, 1);
    background.SetData(new[] { Color.White });
    IsActive = true;
  }

  public void Update(GameTime gameTime)
  {
    if (!IsActive) return;

    _cursorTimer += gameTime.ElapsedGameTime.TotalSeconds;
    if (_cursorTimer >= 0.5)
    {
      _showCursor = !_showCursor;
      _cursorTimer = 0;
    }

    KeyboardState currentKeyState = Keyboard.GetState();
    Keys[] pressedKeys = currentKeyState.GetPressedKeys();

    foreach (Keys key in pressedKeys)
    {
      if (_prevKeyState.IsKeyUp(key))
      {
        HandleKeyPress(key);
      }
    }

    _prevKeyState = currentKeyState;
  }

  private void HandleKeyPress(Keys key)
  {
    if (key == Keys.Back && Text.Length > 0)
    {
      Text = Text.Substring(0, Text.Length - 1);
    }
    else if (key == Keys.Space)
    {
      Text += " ";
    }
    else
    {
      string keyString = key.ToString();
      if (keyString.Length == 1)
      {
        Text += keyString;
      }
    }
  }

  public void Draw(SpriteBatch spriteBatch)
  {
    spriteBatch.Draw(background, Rectangle, IsActive ? Color.White : Color.Gray);
    string display = Text + (IsActive && _showCursor ? "|" : "");
    spriteBatch.DrawString(font, display, new Vector2(Rectangle.X + 5, Rectangle.Y + 5), Color.Black);
  }
}