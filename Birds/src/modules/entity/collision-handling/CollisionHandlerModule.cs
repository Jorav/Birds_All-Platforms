using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.modules.entity.collision_handling;

public class CollisionHandlerModule : ModuleBase
{
  private List<CollisionResponse> _responses = new List<CollisionResponse>();

  public void AddResponse(CollisionResponse response)
  {
    _responses.Add(response);
  }

  public void RemoveResponse(CollisionResponse response)
  {
    _responses.Remove(response);
  }

  protected override void ConfigurePropertySync() { }

  protected override void Update(GameTime gameTime)
  {
    if (container.Collisions.Count > 0)
    {
      foreach (IEntity entity in container.Collisions)
      {
        foreach (CollisionResponse response in _responses)
          response.HandleCollision(this.container, entity);
      }
    }
  }

  public override object Clone()
  {
    var clone = (CollisionHandlerModule)base.Clone();
    clone._responses = new List<CollisionResponse>();
    foreach (var response in _responses)
      clone._responses.Add((CollisionResponse)response.Clone());

    return clone;
  }
}