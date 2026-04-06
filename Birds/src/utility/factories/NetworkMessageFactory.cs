// NetworkMessageFactory.cs
using Birds.src.api.contracts;
using Birds.src.containers.composite;
using Birds.src.containers.composite.blueprints;
using Birds.src.containers.entity;
using Birds.src.factories;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.api.network;

public static class NetworkMessageFactory
{
  public static CompositeSpawnData CreateCompositeSpawnData(CompositeController composite)
  {
    var entities = composite.Entities.Cast<WorldEntity>().ToList();

    var blueprint = BlueprintFactory.CreateBlueprint(entities, "network_temp");

    var spawnData = new CompositeSpawnData
    {
      Id = composite.Id,
      SpawnPosition = composite.Position.Value,
      Entities = blueprint.Entities,
      Connections = blueprint.Connections
    };

    for (int i = 0; i < entities.Count; i++)
    {
      spawnData.ServerEntityIdByBlueprintIndex[i] = entities[i].Id;
    }

    return spawnData;
  }

  public static List<WorldEntity> CreateEntitiesFromSpawnData(CompositeSpawnData spawnData)
  {
    var blueprint = new CompositeBlueprint
    {
      Name = "network_temp",
      Entities = spawnData.Entities,
      Connections = spawnData.Connections
    };

    var entities = BlueprintFactory.CreateFromBlueprint(blueprint, spawnData.SpawnPosition);

    for (int i = 0; i < entities.Count; i++)
    {
      if (spawnData.ServerEntityIdByBlueprintIndex.TryGetValue(i, out string serverId))
      {
        entities[i].Id = serverId;
      }
    }

    return entities;
  }
}
