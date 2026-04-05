using System.Collections.Generic;

namespace Birds.src.api.contracts;

public class GameStateMessage
{
  public long Tick { get; set; }
  public string PlayerId { get; set; }
  public Dictionary<string, EntityStateUpdate> EntityUpdatesPerPlayer { get; set; } = new();
}

public class EntityStateUpdate
{
  public string EntityId { get; set; }
  public float? X { get; set; }
  public float? Y { get; set; }
  public float? VelX { get; set; }
  public float? VelY { get; set; }
  public float? Rotation { get; set; }
}
