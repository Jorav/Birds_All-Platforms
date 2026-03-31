using Birds.src.containers.composite;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.modules.entity;
using Birds.src.utility;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
    if(newEntity is WorldEntity we && we.EntityID == ID_ENTITY.FILLER)
    {
      fillerEntities.Add(newEntity);
    }
    var newLinkModule = newEntity.GetModule<LinkModule>();
    if (newLinkModule == null)
    {
      return;
    }
    var linkModules = container.Entities
        .Where(e => e is WorldEntity we && we.EntityID != ID_ENTITY.FILLER)
        .Select(e => e.GetModule<LinkModule>())
        .Where(linkModule => linkModule != null)
        .Where(linkModule => linkModule != newLinkModule)
        .ToList();
    foreach (LinkModule linkModule in linkModules)
    {
      linkModule.ConnectLinksIfOverlapping(newLinkModule);
      linkModule.Manager = this;
    }
  }

  public void OnEntityRemoved(IEntity entity)
  {
    var linkModule = entity.GetModule<LinkModule>();
    linkModule?.SeverConnections();
    if(entity is WorldEntity we && we.EntityID == ID_ENTITY.FILLER)
    {
      fillerEntities.Remove(we);
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
    foundEntities.Add(e);
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

  public void AddFillerEntities(ID_ENTITY targetID)
  {
    ClearFillerEntities();

    if (!WorldEntityFactory.EntityToSpriteMap.TryGetValue(targetID, out ID_SPRITE ghostSpriteId))
      return;

    var template = WorldEntityFactory.GetEntity(Vector2.Zero, targetID, false);
    var templateLinkModule = template.GetModule<LinkModule>();
    var inboundLink = templateLinkModule?.Links.Count > 0
        ? (templateLinkModule.Links.Count == 4 ? templateLinkModule.Links[2] : templateLinkModule.Links[0])
        : null;

    if (inboundLink == null) { template.Dispose(); return; }

    var realEntities = container.Entities.Where(e => e is WorldEntity we && we.EntityID != ID_ENTITY.FILLER).ToList();

    foreach (var existingEntity in realEntities)
    {
      var existingLinkModule = existingEntity.GetModule<LinkModule>();
      if (existingLinkModule == null) continue;

      foreach (var openLink in existingLinkModule.Links)
      {
        if (!openLink.ConnectionAvailable) continue;
        var filler = WorldEntityFactory.GetEntity(Vector2.Zero, ID_ENTITY.FILLER, false, ghostSpriteId);
        var fillerLinkModule = filler.GetModule<LinkModule>();
        var fillerInbound = fillerLinkModule?.Links.Count > 0
            ? (fillerLinkModule.Links.Count == 4 ? fillerLinkModule.Links[2] : fillerLinkModule.Links[0])
            : null;

        if (fillerInbound != null)
        {
          fillerLinkModule.ConnectAgainstEntity(existingEntity, fillerInbound, openLink);

          bool overlaps = false;
          foreach (var otherEntity in realEntities)
          {
            if (filler.CollidesWith(otherEntity))
            {
              overlaps = true;
              break;
            }
          }
          /**if we want to avoid overlapping entities
          if (!overlaps)
          {
            foreach (var existingFiller in fillerEntities)
            {
              if (filler.CollidesWith(existingFiller))
              {
                overlaps = true;
                break;
              }
            }
          }*/

          if (!overlaps)
          {
            container.Entities.Add(filler);
          }
          else
          {
            fillerLinkModule.SeverConnections();
            filler.Dispose();
          }
        }
        else
        {
          filler.Dispose();
        }
      }
    }
    template.Dispose();
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

  public override object Clone()
  {
    var clone = (LinkManagementModule)this.MemberwiseClone();
    clone.fillerEntities = new();
    return clone;
  }

  public override void Dispose()
  {
    ClearFillerEntities();
    base.Dispose();
  }
}
