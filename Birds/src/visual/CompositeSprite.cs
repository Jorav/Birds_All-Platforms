using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Birds.src.containers.entity;
using System.Linq;

namespace Birds.src.visual;

public class CompositeSprite : ISprite
{
  private struct SpritePart
  {
    public Sprite Sprite;
    public Vector2 LocalOffset;
    public float OriginalScale;
  }

  private readonly List<SpritePart> _parts = new();

  public Vector2 Position { get; set; }
  public float Scale { get; set; } = 1f;
  public Color Color { get; set; } = Color.White;
  public float Alpha { get; set; } = 1f;
  public bool isVisible { get; set; } = true;
  public Vector2 Origin { get; set; }
  public int Width { get; private set; }
  public int Height { get; private set; }

  public CompositeSprite(Vector2 compositeCenter, IEnumerable<IEntity> entities)
  {
    foreach (var entity in entities)
    {
      var drawModule = entity.GetModule<DrawModule>();
      if (drawModule?.Sprite != null)
      {
        var capturedSprite = drawModule.Sprite.Clone();

        _parts.Add(new SpritePart
        {
          Sprite = capturedSprite,
          LocalOffset = entity.Position.Value - compositeCenter,
          OriginalScale = capturedSprite.Scale
        });
      }
    }

    CalculateBounds();
  }

  private void CalculateBounds()
  {
    if (!_parts.Any()) return;

    float minX = _parts.Min(p => p.LocalOffset.X - (p.Sprite.Width * p.OriginalScale / 2));
    float maxX = _parts.Max(p => p.LocalOffset.X + (p.Sprite.Width * p.OriginalScale / 2));
    float minY = _parts.Min(p => p.LocalOffset.Y - (p.Sprite.Height * p.OriginalScale / 2));
    float maxY = _parts.Max(p => p.LocalOffset.Y + (p.Sprite.Height * p.OriginalScale / 2));

    Width = (int)(maxX - minX);
    Height = (int)(maxY - minY);

    Origin = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
  }

  public void Update(GameTime gameTime) { }

  public void Draw(SpriteBatch sb)
  {
    if (!isVisible) return;

    foreach (var part in _parts)
    {
      part.Sprite.Position = this.Position + ((part.LocalOffset - this.Origin) * this.Scale);
      part.Sprite.Scale = part.OriginalScale * this.Scale;
      part.Sprite.Alpha = this.Alpha;
      part.Sprite.Draw(sb);
    }
  }

  public void Dispose()
  {
    foreach (var part in _parts)
    {
      part.Sprite.Dispose();
    }
    _parts.Clear();
  }
}