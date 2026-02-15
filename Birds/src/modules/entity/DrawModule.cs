using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Birds.src.visual;
using Birds.src.utility;
using Birds.src.events;
using Birds.src.modules;
using Birds.src.factories;
using Birds.src.menu;
using Birds.src.modules.shared.collision_detection;
using Birds.src.collision.bounding_areas;
using Birds.src.modules.entity;

public class DrawModule : ModuleBase, IDrawModule
{
  public Sprite Sprite { get; private set; }
  private Vector2 _position;
  public Vector2 Position
  {
    get => _position;
    set
    {
      _position = value;
      if (Sprite != null)
        Sprite.Position = value;
    }
  }
  private float _rotation;
  public float Rotation
  {
    get => _rotation;
    set
    {
      _rotation = value;
      if (Sprite != null)
        Sprite.Rotation = value;
    }
  }
  private Color _color;
  public Color Color
  {
    get => _color;
    set
    {
      _color = value;
      if (Sprite != null)
        Sprite.Color = value;
    }
  }
  private float _scale;
  public float Scale
  {
    get => _scale;
    set
    {
      _scale = value;
      if (Sprite != null)
        Sprite.Scale = value;
    }
  }
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
  }

  protected override void Update(GameTime gameTime)
  {
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
      var cdModule = container.GetModule<CollisionDetectionModule>();
      if (cdModule?.BoundingArea is IRectangle rect)
      {
        var color = container.Collisions.Count > 0 ? Color.Red : Color.Blue;
        var thickness = container.Collisions.Count > 0 ? 3 : 1;
        DrawRectangleOutline(sb, rect.UL, rect.UR, rect.DR, rect.DL, color, thickness);
      }
    }

    if (GameState.DRAW_BC_OUTLINE)
    {
      var cdModule = container.GetModule<BaseCollisionDetectionModule>();
      if (cdModule?.BoundingCircle != null)
      {
        var color = container.Collisions.Count > 0 ? Color.Red : Color.Blue;
        DrawCircleOutline(sb, cdModule.BoundingCircle.Position, cdModule.BoundingCircle.Radius, color, 32, 3);
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