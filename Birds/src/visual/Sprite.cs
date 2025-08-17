using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Birds.src.menu;
using Birds.src.factories;
using Birds.src.utility;

namespace Birds.src.visual;

public class Sprite : IComponent
{
  private Texture2D texture;
  public Texture2D Texture { get { return texture; } set { texture = value; Origin = new Vector2(texture.Width / 2, texture.Height / 2); } }
  public float Rotation { get; set; }
  public Vector2 Position { get; set; }
  public Vector2 Origin { get; set; }
  public int Height { get { return (int)texture.Height; } }
  public int Width { get { return (int)texture.Width; } }
  public Color Color { get; set; }
  public float Alpha { get; set; }
  public bool isVisible = true;

  public float Scale { get; set; }

  public Sprite(Texture2D texture = null, float scale = 1f, float alpha = 1)
  {
    this.Scale = scale;
    if (texture == null)
      texture = SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR];
    else
      this.texture = texture;
    Origin = new Vector2(texture.Width / 2, texture.Height / 2);
    Color = Color.White;
    Alpha = alpha;
  }

  public void Update(GameTime gameTime) { }

  public void Draw(SpriteBatch sb)
  {

    Color c = new Color(Color, alpha: Alpha);
    if (isVisible)
      sb.Draw(texture, Position, null, new Color(Color, alpha: Alpha), Rotation, Origin, Scale, SpriteEffects.None, 0f);
  }

  public void Deprecate()
  {
    SpriteFactory.availableSprites.Push(this);
  }
}
