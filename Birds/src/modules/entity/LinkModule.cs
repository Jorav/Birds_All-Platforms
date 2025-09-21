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
    // Links don't sync properties directly
  }

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    AddLinks();
  }

  protected override void Update(GameTime gameTime)
  {
    // Links update themselves based on entity position/rotation
  }

  private void AddLinks()
  {
    if (Links.Count > 0)
      Links.Clear();

    if (container is IEntity entity)
    {
      // Get width from sprite module
      var spriteModule = entity.GetModule<DrawModule>();
      if (spriteModule != null)
      {
        float width = spriteModule.Sprite.Width;

        Links.Add(new Link(new Vector2(0, -width / 2), entity));
        Links.Add(new Link(new Vector2(width / 2, 0), entity));
        Links.Add(new Link(new Vector2(0, width / 2), entity));
        Links.Add(new Link(new Vector2(-width / 2, 0), entity));
      }
    }
  }

  public void ConnectTo(IEntity eConnectedTo, Link lConnectedTo)
  {
    if (Links.Count > 0 && Links[0] != null && container is IEntity entity)
    {
      lConnectedTo.SeverConnection();
      var internalRotation = Links[0].ConnectTo(lConnectedTo);
      entity.Rotation.Value = eConnectedTo.Rotation.Value - internalRotation;
      entity.Position.Value = lConnectedTo.ConnectionPosition;
    }
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
}

