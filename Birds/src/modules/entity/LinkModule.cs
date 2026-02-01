using Birds.src.containers.composite;
using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.modules.entity;

public class LinkModule : ModuleBase
{
  public List<Link> Links { get; private set; } = new List<Link>();
  public float InternalRotation { get; set; } = 0f;

  protected override void ConfigurePropertySync()
  {
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    AddLinks();
  }

  protected override void Update(GameTime gameTime)
  {
  }

  private void AddLinks()
  {
    if (Links.Count > 0)
      Links.Clear();

    if (container is IEntity entity)
    {
      var spriteModule = entity.GetModule<DrawModule>();
      if (spriteModule != null)
      {
        float width = spriteModule.Sprite.Width;

        Links.Add(new Link(new Vector2(width / 2, 0), entity));   // Right (0)
        Links.Add(new Link(new Vector2(0, width / 2), entity));   // Down (1)
        Links.Add(new Link(new Vector2(-width / 2, 0), entity));  // Left (2)
        Links.Add(new Link(new Vector2(0, -width / 2), entity));  // Up (3)
      }
    }
  }

  public void ConnectAgainst(IEntity otherEntity, Link myLink, Link otherLink)
  {
    otherLink.SeverConnection();
    myLink.SeverConnection();
    float otherLinkWorldAngle = otherLink.LinkRotation + otherEntity.Rotation.Value;
    float targetLinkWorldAngle = otherLinkWorldAngle + MathHelper.Pi;
    container.Rotation.Value = MathHelper.WrapAngle(targetLinkWorldAngle - myLink.LinkRotation);
    container.Position.Value = otherLink.ConnectionPosition;
    myLink.ConnectTo(otherLink);
  }

  public void SeverConnection(IEntity e)
  {
    foreach (var link in Links)
    {
      if (!link.ConnectionAvailable && link.connection.Entity == e)
      {
        link.SeverConnection();
      }
    }
  }

  public void SeverConnections()
  {
    foreach (var link in Links)
    {
      if (!link.ConnectionAvailable)
      {
        link.SeverConnection();
      }
    }
  }

  public void UpdateScale(float scale)
  {
    foreach (var link in Links)
    {
      link.Scale = scale;
    }
  }

  public override object Clone()
  {
    var cloned = new LinkModule();
    cloned.InternalRotation = this.InternalRotation;
    return cloned;
  }

  public void Dispose()
  {
    base.Dispose();
    foreach(Link l in Links)
    {
      l.SeverConnection();
    }
  }
}
