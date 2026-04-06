using Birds.src.containers.composite.blueprints.parts;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.api.contracts;

public class EntitySpawnData
{
  public string Id { get; set; }
  public ID_ENTITY EntityType { get; set; }
  public Vector2 Position { get; set; }
  public float Rotation { get; set; }
}

public class CompositeSpawnData
{
  public string Id { get; set; }
  public Vector2 SpawnPosition { get; set; }
  public List<EntityPlacement> Entities { get; set; } = new();
  public List<Connection> Connections { get; set; } = new();
  public Dictionary<int, string> ServerEntityIdByBlueprintIndex { get; set; } = new();
}

public class ControllerSpawnMessage
{
  public string Id { get; set; }
  public ID_CONTROLLER ControllerType { get; set; }
  public Vector2 Position { get; set; }
  public List<EntitySpawnData> DirectEntities { get; set; } = new();
  public List<CompositeSpawnData> Composites { get; set; } = new();
}
