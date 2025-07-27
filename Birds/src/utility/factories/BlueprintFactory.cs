using Birds.src.controllers.composite.blueprints;
using Birds.src.controllers.composite.blueprints.parts;
using Birds.src.entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Birds.src.factories;
public static class BlueprintFactory  //ADD REUSE HANDLING FROM OTHER FACTORIES
{
  public static List<WorldEntity> CreateFromBlueprint(CompositeBlueprint blueprint, Vector2 spawnPosition) //RETURN A COMPOSITE ENTITY
  {
    if (blueprint == null || blueprint?.Entities == null || blueprint?.Connections == null || blueprint.Entities.Count == 0)
      throw new Exception("Faulty blueprint input");

    var entities = new List<WorldEntity>();
    var entityLookup = new Dictionary<int, WorldEntity>();

    foreach (var placement in blueprint.Entities)
    {
      var entity = EntityFactory.GetEntity(spawnPosition, placement.EntityType);
      entities.Add(entity);
      entityLookup[placement.Id] = entity;
    }

    ConnectEntities(blueprint.Connections, entityLookup);

    return entities;
  }

  private static void ConnectEntities(List<Connection> connections, Dictionary<int, WorldEntity> entityLookup)
  {
    foreach (var connection in connections)
    {
      var entity1 = entityLookup[connection.EntityId1];
      var entity2 = entityLookup[connection.EntityId2];

      var link1 = entity1.Links[connection.LinkIndex1];
      var link2 = entity2.Links[connection.LinkIndex2];

      if (link1.ConnectionAvailable && link2.ConnectionAvailable)
      {
        entity1.ConnectTo(entity2, link2); //FIX POSITION OF ENTITIES
      }
    }
  }

  public static CompositeBlueprint CreateBlueprint(List<WorldEntity> entities, string blueprintName)
  {
    if (entities == null || entities.Count == 0)
    {
      throw new Exception("Invalid entity formation to save");
    }

    var blueprint = new CompositeBlueprint
    {
      Name = blueprintName,
      Entities = new List<EntityPlacement>(),
      Connections = new List<Connection>()
    };

    var entityToId = CreateWorldEntityIds(entities);
    blueprint.Entities = CreateEntityPlacements(entities, entityToId);

    blueprint.Connections = CreateConnections(entities, entityToId);

    return blueprint;
  }
  private static Dictionary<WorldEntity, int> CreateWorldEntityIds(List<WorldEntity> entities)
  {
    var entityToId = new Dictionary<WorldEntity, int>();

    for (int i = 0; i < entities.Count; i++)
    {
      entityToId[entities[i]] = i;
    }

    return entityToId;
  }
  private static List<EntityPlacement> CreateEntityPlacements(List<WorldEntity> entities, Dictionary<WorldEntity, int> entityToId)
  {
    var placements = new List<EntityPlacement>();

    foreach (var entity in entities)
    {
      placements.Add(new EntityPlacement
      {
        Id = entityToId[entity],
        EntityType = entity.EntityID
      });
    }
    return placements;
  }
  private static List<Connection> CreateConnections(List<WorldEntity> entities, Dictionary<WorldEntity, int> entityToId)
  {
    var connections = new List<Connection>();

    foreach (var entity in entities)
    {
      var entityId = entityToId[entity];

      for (int linkIndex = 0; linkIndex < entity.Links.Count; linkIndex++)
      {
        var link = entity.Links[linkIndex];

        if (link.ConnectionAvailable)
        {
          continue;
        }

        var connectedEntity = link.connection.Entity;
        var connectedEntityId = entityToId[connectedEntity];

        if (entityId > connectedEntityId)
        {
          continue;
        }

        var connectedLinkIndex = connectedEntity.Links.IndexOf(link.connection);
        connections.Add(new Connection
        {
          EntityId1 = entityId,
          EntityId2 = connectedEntityId,
          LinkIndex1 = linkIndex,
          LinkIndex2 = connectedLinkIndex
        });
      }
    }
    return connections;
  }

}
