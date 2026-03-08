using System.Collections.Generic;

namespace Birds.src.collision.BVH;

public class ListCollisionStructure : ICollisionStructure
{
  private List<ICollidable> entities = new List<ICollidable>();

  public void Build(List<ICollidable> entities)
  {
    this.entities = entities;
  }

  public void AddCollisionsToEntities(ICollidable other)
  {
    foreach (var entity in entities)
    {
      entity.AddCollisionsToEntities(other);
    }
  }

  public void AddInternalCollisions()
  {
    for (int i = 0; i < entities.Count; i++)
    {
      for (int j = i + 1; j < entities.Count; j++)
      {
        entities[i].AddCollisionsToEntities(entities[j]);
      }
    }
  }

  public void Dispose()
  {
    entities.Clear();
  }
}