using Birds.src.containers.composite;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.composite;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.modules.entity;

public class LinkModule : ModuleBase
{
  public List<Link> Links { get; private set; } = new List<Link>();
  public float InternalRotation { get; set; } = 0f;
  public LinkManagementModule Manager { get; set; }

  private List<LinkConfiguration> _linkConfigurations = new List<LinkConfiguration>();

  protected override void ConfigurePropertySync()
  {
  }

  public void SetLinkConfigurations(List<LinkConfiguration> configurations)
  {
    _linkConfigurations = configurations;
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
      foreach (var config in _linkConfigurations)
      {
        Vector2 offset = config.GetOffset(container.Width.Value, container.Height.Value);
        Links.Add(new Link(offset, entity));
      }
    }
  }

  public void UpdateScale(float scale)
  {
    if (container is IEntity entity)
    {
      for (int i = 0; i < Links.Count && i < _linkConfigurations.Count; i++)
      {
        Vector2 newOffset = _linkConfigurations[i].GetOffset(container.Width.Value, container.Height.Value);
        Links[i].UpdateOffset(newOffset);
      }
    }
  }

  public void ConnectAgainstEntity(IEntity otherEntity, Link myLink, Link otherLink)
  {
    otherLink.SeverConnection();
    myLink.SeverConnection();

    float otherLinkWorldAngle = otherLink.LinkRotation + otherEntity.Rotation.Value;
    float targetLinkWorldAngle = otherLinkWorldAngle + MathHelper.Pi;

    container.Rotation.Value = MathHelper.WrapAngle(targetLinkWorldAngle - myLink.LinkRotation);
    container.Position.Value = otherLink.GetConnectionPosition(myLink);

    myLink.ConnectTo(otherLink);
  }

  public void ConnectLinksIfOverlapping(LinkModule otherModule)
  {
    foreach (Link link in Links)
    {
      if (!link.ConnectionAvailable)
        continue;

      foreach (Link linkOther in otherModule.Links)
      {
        if (!linkOther.ConnectionAvailable)
          continue;

        Vector2 myConnectionPos = link.GetConnectionPosition(linkOther);
        Vector2 otherConnectionPos = linkOther.GetConnectionPosition(link);

        if (container.Contains(otherConnectionPos) && otherModule.container.Contains(myConnectionPos))
        {
          link.ConnectTo(linkOther);
        }
      }
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
  public (Link myLink, Link otherLink)? GetFirstConnection()
  {
    foreach (var link in Links)
    {
      if (!link.ConnectionAvailable)
      {
        return (link, link.connection);
      }
    }
    return null;
  }

  public bool ReplaceWithNewEntity(IEntity oldEntity, IEntity newEntity, IModuleContainer parentContainer)
  {
    var connection = GetFirstConnection();
    if (connection == null)
    {
      return false;
    }

    var (oldLink, connectedLink) = connection.Value;

    SeverConnections();
    parentContainer.Entities.Remove(oldEntity);

    var newLinkModule = newEntity.GetModule<LinkModule>();
    if (newLinkModule == null || newLinkModule.Links.Count == 0)
    {
      parentContainer.Entities.Add(oldEntity);
      oldLink.ConnectTo(connectedLink);
      return false;
    }

    var newLink = newLinkModule.Links[0];
    newLinkModule.ConnectAgainstEntity(connectedLink.Entity, newLink, connectedLink);

    foreach (IEntity e in parentContainer.Entities)
    {
      if (e.CollidesWith(newEntity))
      {
        newLinkModule.SeverConnections();
        parentContainer.Entities.Add(oldEntity);
        oldLink.ConnectTo(connectedLink);
        return false;
      }
    }

    parentContainer.Entities.Add(newEntity);
    return true;
  }
  public override object Clone()
  {
    var cloned = new LinkModule();
    cloned.InternalRotation = this.InternalRotation;
    cloned._linkConfigurations = new List<LinkConfiguration>();
    foreach (var config in _linkConfigurations)
    {
      cloned._linkConfigurations.Add(new LinkConfiguration
      {
        Angle = config.Angle,
        Distance = config.Distance
      });
    }
    return cloned;
  }

  public override void Dispose()
  {
    base.Dispose();
    foreach (Link l in Links)
    {
      l.Dispose();
    }
    Links.Clear();
    Manager = null;
  }
}