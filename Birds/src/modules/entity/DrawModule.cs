using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using System;
using Birds.src.visual;
using Birds.src.utility;
using Birds.src.events;
using Birds.src.modules;
using Birds.src.modules.shared.bounding_area;
using Birds.src.factories;
using Birds.src.menu;

public class DrawModule : ModuleBase, IDrawModule
{
  public Sprite Sprite { get; private set; }
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
  public Color Color { get; set; }
  public float Scale { get; set; }
  public float Width { get; set; }
  public float Height { get; set; }

  private static Texture2D _pixel;

  public DrawModule(ID_ENTITY entityId, float scale = 1f)
  {
    Sprite = SpriteFactory.GetSprite(entityId, Vector2.Zero, scale);
  }

  public static void InitializePixel(GraphicsDevice graphicsDevice)
  {
    if (_pixel == null)
    {
      _pixel = new Texture2D(graphicsDevice, 1, 1);
      _pixel.SetData(new[] { Color.White });
    }
  }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
    ReadSync(() => Rotation, container.Rotation);
    ReadWriteSync(() => Color, container.Color);
    ReadWriteSync(() => Scale, container.Scale);
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

  public static void DrawCircleOutline(SpriteBatch sb, Vector2 center, float radius, Color color, int segments = 32, int thickness = 1)
  {
    float angleStep = MathHelper.TwoPi / segments;
    for (int i = 0; i < segments; i++)
    {
      float angle1 = i * angleStep;
      float angle2 = (i + 1) * angleStep;

      Vector2 point1 = center + new Vector2((float)Math.Cos(angle1), (float)Math.Sin(angle1)) * radius;
      Vector2 point2 = center + new Vector2((float)Math.Cos(angle2), (float)Math.Sin(angle2)) * radius;

      DrawLine(sb, point1, point2, color, thickness);
    }
  }

  public static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, int thickness)
  {
    Vector2 edge = end - start;
    float angle = (float)Math.Atan2(edge.Y, edge.X);
    sb.Draw(_pixel, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
            null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
  }

  public void Draw(SpriteBatch sb)
  {
    Sprite?.Draw(sb);
    if (GameState.DRAW_OBB_OUTLINE)
    {
      var obbModule = container.GetModule<OBBCollisionDetectionModule>();
      if (obbModule?.OBB != null)
      {
        if (container.Collisions.Count > 0)
        {
          DrawRectangleOutline(sb, obbModule.OBB.UL, obbModule.OBB.UR, obbModule.OBB.DR, obbModule.OBB.DL, Color.Red, 3);
        }
        else
        {
          DrawRectangleOutline(sb, obbModule.OBB.UL, obbModule.OBB.UR, obbModule.OBB.DR, obbModule.OBB.DL, Color.Blue, 1);
        }
      }
    }
    if (GameState.DRAW_BC_OUTLINE)
    {
      var bcModule = container.GetModule<BCCollisionDetectionModule>();
      if (bcModule?.BoundingCircle != null)
      {
        if (container.Collisions.Count > 0)
        {
          DrawModule.DrawCircleOutline(sb, bcModule.BoundingCircle.Position, bcModule.BoundingCircle.Radius, Color.Red, 32, 3);
        }
        else
        {
          DrawModule.DrawCircleOutline(sb, bcModule.BoundingCircle.Position, bcModule.BoundingCircle.Radius, Color.Blue);
        }
      }
    }
  }

  public override object Clone()
  {
    var cloned = (DrawModule)this.MemberwiseClone();
    cloned.Sprite = new Sprite(this.Sprite.Texture, this.Scale);
    return cloned;
  }

  public static void DrawRectangleOutline(SpriteBatch sb, Vector2 ul, Vector2 ur, Vector2 dr, Vector2 dl, Color color, int thickness)
  {
    DrawLine(sb, ul, ur, color, thickness);
    DrawLine(sb, ur, dr, color, thickness);
    DrawLine(sb, dr, dl, color, thickness);
    DrawLine(sb, dl, ul, color, thickness);
  }
}