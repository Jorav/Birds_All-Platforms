using System.Collections.Generic;
using System.Linq;
using Birds.src.collision.BVH;
using Birds.src.modules.collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.events;
using Birds.src.collision;

namespace Birds.src.containers.controller;

public class GameController
{
  public static List<Controller> controllers = new();
  private AABBTree collisionManager = new();

  public void Add(Controller c)
  {
    controllers.Add(c);
    UpdateGlobalCollisionTree();
  }

  private void UpdateGlobalCollisionTree()
  {
    var collisionHandlers = controllers
        .Select(c => c.GetModule<GroupCollisionHandlerModule>())
        .Where(handler => handler != null)
        .Cast<ICollidable>()
        .ToList();

    if (collisionHandlers.Count > 0)
    {
      collisionManager.UpdateTree(collisionHandlers);
    }
  }

  public void Update(GameTime gameTime)
  {
    foreach (Controller c in controllers)
      c.Update(gameTime);
    UpdateGlobalCollisionTree();
    collisionManager.Update(gameTime);
  }

  public void Draw(SpriteBatch sb)
  {
    foreach (Controller c in controllers)
    {
      c.Draw(sb);
    }
  }
}

