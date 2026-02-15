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
  public HashSet<IEntity> fillerEntities = new HashSet<IEntity>();

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
  }

  protected override void Update(GameTime gameTime)
  {
  }

  public void OnEntityAdded(IEntity newEntity) 
  {
    var newLinkModule = newEntity.GetModule<LinkModule>();
    if (newLinkModule == null)
    {
      return;
    }
    var linkModules = container.Entities
        .Where(e => e is WorldEntity we && !we.IsFiller)
        .Select(e => e.GetModule<LinkModule>())
        .Where(linkModule => linkModule != null)
        .Where(linkModule => linkModule != newLinkModule)
        .ToList();
    foreach (LinkModule linkModule in linkModules)
    {
      linkModule.ConnectLinksIfOverlapping(newLinkModule);//THIS ISNT WORKING IT SEEMS
    }
  }

  public void OnEntityRemoved(IEntity entity)
  {
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

        var fillerEntity = WorldEntityFactory.GetEntity(link.ConnectionPosition, ID_ENTITY.FILLER, false);
        fillerEntity.Mass.Value = 1f;
        var fillerLinkModule = fillerEntity.GetModule<LinkModule>();
        var backLink = fillerLinkModule.Links[2];
        if (backLink != null)
          fillerLinkModule.ConnectEntityAgainstThis(entity, backLink, link);

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
