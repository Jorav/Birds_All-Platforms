using Birds.src.entities;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.modules.entity;
public class LinkModule : ModuleBase
{
  public List<WorldEntity.Link> Links { get; private set; } = new List<WorldEntity.Link>();
  public float Width { get; set; } = 32f;

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

    // Assuming this is for a WorldEntity
    if (container is WorldEntity entity)
    {
      Links.Add(new WorldEntity.Link(new Vector2(0, -Width / 2), entity));
      Links.Add(new WorldEntity.Link(new Vector2(Width / 2, 0), entity));
      Links.Add(new WorldEntity.Link(new Vector2(0, Width / 2), entity));
      Links.Add(new WorldEntity.Link(new Vector2(-Width / 2, 0), entity));
    }
  }

  public void ConnectTo(WorldEntity eConnectedTo, WorldEntity.Link lConnectedTo)
  {
    if (Links.Count > 0 && Links[0] != null && container is WorldEntity entity)
    {
      lConnectedTo.SeverConnection();
      var internalRotation = Links[0].ConnectTo(lConnectedTo);
      //entity.EntityRotation = eConnectedTo.EntityRotation - internalRotation;
      //entity.EntityPosition = lConnectedTo.ConnectionPosition;
    }
  }

  public void SeverConnection(WorldEntity e)
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
}