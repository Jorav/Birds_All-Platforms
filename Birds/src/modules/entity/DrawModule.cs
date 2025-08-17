using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.utility;
using System;

namespace Birds.src.modules.entity;

public class DrawModule : ModuleBase, IDrawModule
{
  public Sprite Sprite { get; private set; }
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
  public Color Color { get; set; }
  public float Scale { get; set; }
  public float Width { get; set; }
  public float Height { get; set; }
  public float Radius { get; set; }

  public DrawModule(ID_ENTITY entityId, float scale = 1f)
  {
    Sprite = SpriteFactory.GetSprite(entityId, Vector2.Zero, scale);
  }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
    ReadSync(() => Rotation, container.Rotation);
    ReadWriteSync(() => Color, container.Color);
    ReadWriteSync(() => Scale, container.Scale);
    WriteSync(() => Radius, container.Radius);
    WriteSync(() => Width, container.Width);
    WriteSync(() => Height, container.Height);
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    Scale = Sprite.Scale;
    Color = Sprite.Color;
    Width = Sprite.Width;
    Height = Sprite.Height;
    Radius = (float)Math.Sqrt(Math.Pow(Sprite.Width / 2, 2) + Math.Pow(Sprite.Height / 2, 2));
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

