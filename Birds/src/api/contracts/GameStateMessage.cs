using System.Collections.Generic;

namespace Birds.src.api.contracts;

public class GameStateMessage
{
  public long Tick { get; set; }
  public string PlayerId { get; set; }
  public Dictionary<string, EntityStateUpdate> EntityUpdatesPerPlayer { get; set; } = new();
}