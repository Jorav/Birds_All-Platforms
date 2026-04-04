namespace Birds.src.api.contracts;

public class EntityStateUpdate
{
  public string EntityId { get; set; }
  public float? X { get; set; }
  public float? Y { get; set; }
  public float? VelX { get; set; }
  public float? VelY { get; set; }
  public float? Rotation { get; set; }
}
