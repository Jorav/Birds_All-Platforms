using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
      container.Collisions.Clear();
    }
  }
}