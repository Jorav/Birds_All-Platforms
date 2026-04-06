using Microsoft.Xna.Framework;
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
  public Vector2? Position { get; set; }
  public Vector2? Velocity { get; set; }
  public float? Rotation { get; set; }
}