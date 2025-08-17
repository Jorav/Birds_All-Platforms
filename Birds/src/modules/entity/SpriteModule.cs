using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.utility;

namespace Birds.src.modules.entity;

public class SpriteModule : ControllerModule
{
  public Sprite Sprite { get; private set; }
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
  public Color Color { get; set; }
  public float Scale { get; set; }
  public float Width { get; set; }
  public float Height { get; set; }

  public SpriteModule(ID_ENTITY entityId, float scale = 1f)
  {
    Sprite = SpriteFactory.GetSprite(entityId, Vector2.Zero, scale);
  }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
    ReadSync(() => Rotation, container.Rotation);
    ReadSync(() => Color, container.Color);
    ReadSync(() => Scale, container.Scale);

    WriteSync(() => Color, container.Color);
    WriteSync(() => Scale, container.Scale);
    WriteSync(() => Width, container.Width);
    WriteSync(() => Height, container.Height);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    container.Scale.Value = Scale;
    container.Color.Value = Color;
    container.Width.Value = Width;
    container.Height.Value = Height;
    UpdateSpriteFromContainer();
  }

  protected override void Update(GameTime gameTime)
  {
    UpdateSpriteFromContainer();
  }

  private void UpdateSpriteFromContainer()
  {
    if (Sprite != null)
    {
      Sprite.Position = Position;
      Sprite.Rotation = Rotation;
      Sprite.Color = Color;
      Sprite.Scale = Scale;
    }
  }

  public void Draw(SpriteBatch sb)
  {
    Sprite?.Draw(sb);
  }
}

