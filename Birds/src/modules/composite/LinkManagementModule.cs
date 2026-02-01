using Birds.src.containers.composite;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.modules.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.composite;

public class LinkManagementModule : ModuleBase, IEntityCollectionListener
{
  private HashSet<IEntity> fillerEntities = new HashSet<IEntity>();

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
  }

  protected override void Update(GameTime gameTime)
  {
  }

  public void OnEntityAdded(IEntity entity)
  {
  }

  public void OnEntityRemoved(IEntity entity)
  {
  }

  protected void ConnectToOthers(IEntity entity)
  {
    if (container.Entities.Count <= 0)
    {
      return;
    }

    var linkModule = entity.GetModule<LinkModule>();
    if (linkModule == null) return;

    foreach (IEntity entityOther in container.Entities)
    {
      if (entity == entityOther)
      {
        continue;
      }

      var otherLinkModule = entityOther.GetModule<LinkModule>();
      if (otherLinkModule == null)
      {
        continue;
      }
      ConnectMatchingPositionLinks(linkModule, otherLinkModule);
    }
  }

  private void ConnectMatchingPositionLinks(LinkModule module1, LinkModule module2)
  {
    const float positionTolerance = 5f;

    foreach (var link1 in module1.Links)
    {
      if (!link1.ConnectionAvailable) continue;

      foreach (var link2 in module2.Links)
      {
        if (!link2.ConnectionAvailable) continue;

        float distance = Vector2.Distance(link1.ConnectionPosition, link2.ConnectionPosition);
        if (distance <= positionTolerance)
        {
          link1.ConnectTo(link2);
          return;
        }
      }
    }
  }

  public List<HashSet<IEntity>> GetDisconnectedGroups()
  {
    var groups = new List<HashSet<IEntity>>();
    var visited = new HashSet<IEntity>();
    var worldEntities = container.Entities.OfType<IEntity>().ToList();

    foreach (IEntity entity in worldEntities)
    {
      if (!visited.Contains(entity))
      {
        var connectedGroup = GetConnectedEntities(entity, new HashSet<IEntity>());
        groups.Add(connectedGroup);
        visited.UnionWith(connectedGroup);
      }
    }
    return groups;
  }

  private HashSet<IEntity> GetConnectedEntities(IEntity e, HashSet<IEntity> foundEntities)
  {
    var linkModule = e.GetModule<LinkModule>();
    if (linkModule == null) return foundEntities;

    foreach (var l in linkModule.Links)
    {
      if (!l.ConnectionAvailable && !foundEntities.Contains(l.connection.Entity))
      {
        foundEntities.Add(l.connection.Entity);
        GetConnectedEntities(l.connection.Entity, foundEntities);
      }
    }
    return foundEntities;
  }

  public void AddFillerEntities()
  {
    ClearFillerEntities();

    foreach (var entity in container.Entities.ToList())
    {
      var entityLinkModule = entity.GetModule<LinkModule>();
      if (entityLinkModule == null) continue;

      foreach (var link in entityLinkModule.Links)
      {
        if (!link.ConnectionAvailable)
          continue;

        var fillerEntity = EntityFactory.GetEntity(link.ConnectionPosition, ID_ENTITY.FILLER, false);
        var fillerLinkModule = fillerEntity.GetModule<LinkModule>();
        var backLink = fillerLinkModule.Links[2];
        if (backLink != null)
          fillerLinkModule.ConnectAgainst(entity, backLink, link);

        bool overlaps = false;
        foreach (var existingEntity in container.Entities)
        {
          if (fillerEntity.CollidesWith(existingEntity))
          {
            overlaps = true;
            break;
          }
        }

        if (!overlaps)
        {
          fillerEntities.Add(fillerEntity);
          container.Entities.Add(fillerEntity);
        }
        else
        {
          fillerLinkModule.SeverConnections();
          fillerEntity.Dispose();
        }
      }
    }
  }

  public void ClearFillerEntities()
  {
    foreach (var fillerEntity in fillerEntities)
    {
      var linkModule = fillerEntity.GetModule<LinkModule>();
      if (linkModule != null)
      {
        foreach (var link in linkModule.Links)
        {
          link.SeverConnection();
        }
      }
      container.Entities.Remove(fillerEntity);
      fillerEntity.Dispose();
    }
    fillerEntities.Clear();
  }
}
