using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.composite;

public class LinkManagementModule : ModuleBase, IEntityCollectionListener
{
  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);

    foreach (var entity in container.Entities)
    {
      ConnectToOthers(entity);
    }
  }

  protected override void Update(GameTime gameTime)
  {
  }

  public void OnEntityAdded(IEntity entity)
  {
    ConnectToOthers(entity);
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
      otherLinkModule.ConnectTo(linkModule);
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
}
