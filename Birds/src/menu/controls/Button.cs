using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.visual;
using System;
using Birds.src.utility;
using Birds.src.player;

namespace Birds.src.menu.controls;

public class Button : IComponent
{
  #region Fields
  protected internal bool isHovering;
  protected ISprite sprite;
  private bool isBeingHeld;
  private double currentHoldTime;
  private bool longPressTriggered;
  private const double LongPressThreshold = 600;
  #endregion

  #region Properties
  public event EventHandler Click;
  public event EventHandler LongPress;

  public bool Clicked { get; private set; }
  protected Vector2 position;
  public virtual Vector2 Position { get { return position; } set { sprite.Position = value; TextNew.Position = value + new Vector2(Rectangle.Width / 2, Rectangle.Height / 2); position = value; } }
  protected float scale = 1f;
  public virtual float Scale
  {
    get { return scale; }
    set { sprite.Scale = value; scale = value; }
  }
  public Vector2 Dimensions { get { return new Vector2(sprite.Width, sprite.Height); } }
  public Rectangle Rectangle
  {
    get { return new Rectangle((int)Position.X, (int)Position.Y, (int)(sprite.Width * scale), (int)(sprite.Height * scale)); }
  }
  public String Text { get { return TextNew.Text; } set { TextNew.Text = value; } }
  public FadingText TextNew { get; set; }
  private Input input{ get; set; }
  #endregion

  #region Methods
  public Button(ISprite sprite, Input input, SpriteFont font = null, String text = null)
  {
    this.sprite = sprite;
    position = sprite.Position;
    sprite.Origin = Vector2.Zero;
    TextNew = new FadingText(text, Position, font);
    TextNew.IsVisible = true;
    this.input = input;
  }

  public bool IsHovering()
  {
    Vector2 pos = input.ScreenPosition;
    Rectangle mouseRectangle = new Rectangle(((int)Math.Round(pos.X)), ((int)Math.Round(pos.Y)), 1, 1);
    return mouseRectangle.Intersects(Rectangle);
  }

  public virtual void Update(GameTime gameTime)
  {
    isHovering = false;
    HandleInput(gameTime);
  }

  protected virtual void HandleInput(GameTime gameTime)
  {
    if (IsHovering())
    {
      isHovering = true;

      if (input.WasJustPressed)
      {
        isBeingHeld = true;
        currentHoldTime = 0;
        longPressTriggered = false;
      }
    }
    else if (isBeingHeld)
    {
      isBeingHeld = false;
    }

    if (isBeingHeld)
    {
      if (input.IsPressed)
      {
        currentHoldTime += gameTime.ElapsedGameTime.TotalMilliseconds;

        if (currentHoldTime >= LongPressThreshold && !longPressTriggered)
        {
          longPressTriggered = true;
          LongPress?.Invoke(this, EventArgs.Empty);
        }
      }
      else
      {
        if (!longPressTriggered && IsHovering())
        {
          InvokeEvent(new EventArgs());
        }

        isBeingHeld = false;
        longPressTriggered = false;
        currentHoldTime = 0;
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

    sprite.Draw(spritebatch);

    if (!string.IsNullOrEmpty(Text))
    {
      TextNew.Draw(spritebatch);
    }
  }
  #endregion
}