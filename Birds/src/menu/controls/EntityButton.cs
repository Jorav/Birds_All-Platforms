using Birds.src.visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.menu.controls;

public class EntityButton : Button
{
  public Sprite entitySprite { get; private set; }
  float hullFactor = 1;
  public bool IsClicked { get; set; }
  public override Vector2 Position { get { return position; } set { sprite.Position = value; entitySprite.Position = value + new Vector2(sprite.Width * scale / 2, sprite.Height * scale / 2); position = value; } }
  public override float Scale { get { return scale; } set { sprite.Scale = value; entitySprite.Scale = value * hullFactor; scale = value; Position = position; } } //doesnt work with text
  public EntityButton(Sprite entitySprite, Sprite backgroundSprite, bool displaysHull = false, SpriteFont font = null) : base(backgroundSprite, font)
  {
    this.entitySprite = entitySprite;
    if (displaysHull)
      hullFactor = 0.75f;
    //this.entitySprite.Origin = Vector2.Zero;//-Scale*new Vector2(sprite.Width / 2, sprite.Height / 2);//Vector2.Zero;
  }

  public override void Draw(SpriteBatch spritebatch)
  {
    bool previousIsHovering = isHovering;
    if (IsClicked)
      isHovering = true;

    base.Draw(spritebatch);
    entitySprite.Draw(spritebatch);

    isHovering = previousIsHovering;
  }
}
