using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.composite;

public class LinkManagementModule : ModuleBase
{
  private HashSet<IEntity> processedEntities = new HashSet<IEntity>();

  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);

    if (container.Entities.Count > 0)
    {
      RebuildLinks();
    }
  }

  protected override void Update(GameTime gameTime)
  {
    var unprocessedEntities = container.Entities.Where(e => !processedEntities.Contains(e)).ToList();

    foreach (var entity in unprocessedEntities)
    {
      if (entity is WorldEntity we)
      {
        ConnectToOthers(we);
        processedEntities.Add(entity);
      }
    }
  }

  public void AddEntity(IEntity entity)
  {
    ConnectToOthers(entity);
    processedEntities.Add(entity);
    container.Entities.Add(entity);
  }

  public void RemoveEntity(IEntity entity)
  {
    processedEntities.Remove(entity);
    container.Entities.Remove(entity);
  }

  public void SetEntities(List<IEntity> newEntities)
  {
    if (newEntities == null || newEntities.Count == 0)
      return;

    foreach (IEntity entity in newEntities)
      AddEntity(entity);
  }

  private void RebuildLinks()
  {
    processedEntities.Clear();

    var worldEntities = container.Entities.OfType<IEntity>().ToList();
    foreach (var entity in worldEntities)
    {
      if (true)//!entity.IsFiller)
      {
        ConnectToOthers(entity);
        processedEntities.Add(entity);
      }
    }
  }

  protected void ConnectToOthers(IEntity entity)
  {
    if (container.Entities.Count <= 0)// || entity.IsFiller
    {
      return;
    }

    var linkModule = entity.GetModule<LinkModule>();
    if (linkModule == null) return;

    foreach (IEntity entityOther in container.Entities)
    {
      if (entity == entityOther)// || e.IsFiller
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
    List<HashSet<IEntity>> sets = new List<HashSet<IEntity>>();
    var worldEntities = container.Entities.OfType<IEntity>().ToList();

    foreach (IEntity e in worldEntities)
    {
      bool containsEntity = sets.Any(s => s.Contains(e));
      if (!containsEntity)
      {
        HashSet<IEntity> set = new HashSet<IEntity>();
        set.Add(e);
        GetConnectedEntities(e, set);
        sets.Add(set);
      }
    }
    return sets;
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

  public override object Clone()
  {
    return new LinkManagementModule();
  }
}