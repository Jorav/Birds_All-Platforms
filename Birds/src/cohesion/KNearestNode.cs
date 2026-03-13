using Birds.src.containers.entity;
using Microsoft.Xna.Framework;

namespace Birds.src.cohesion;

public class KNearestNode
{
  public IEntity Entity { get; set; }
  public KNearestNode Left { get; set; }
  public KNearestNode Right { get; set; }
  public KNearestNode Parent { get; set; }
  public float SubtreeRadius { get; set; }
  public Vector2 CenterOfMass { get; set; }
  public float TotalMass { get; set; }
  public int EntityCount { get; set; }

  public bool IsLeaf => Entity != null;
}
