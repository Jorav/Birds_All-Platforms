using System.Collections.Generic;
using System.Linq;
using Birds.src.collision.BVH;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.events;
using Birds.src.collision;
using Birds.src.modules.shared.collision_detection;

namespace Birds.src.containers.controller;

public class GameController
{
  public static List<Controller> controllers = new();
  private AABBTree collisionManager = new();

  public void Add(Controller c)
  {
    controllers.Add(c);
  }

  public void Update(GameTime gameTime)
  {
    foreach (Controller c in controllers)
    {
      c.Update(gameTime);
    }
    UpdateGlobalCollisionTree();
    collisionManager.AddInternalCollisions();
  }

  private void UpdateGlobalCollisionTree()
  {
    var collisionDetectors = controllers
        .Select(c => c.GetModule<GroupCollisionDetectionModule>())
        .Where(handler => handler != null)
        .Cast<ICollidable>()
        .ToList();

    if (collisionDetectors.Count > 0)
    {
      collisionManager.BuildTree(collisionDetectors);
    }
  }

  public void Draw(SpriteBatch sb)
  {
    foreach (Controller c in controllers)
    {
      c.Draw(sb);
    }
  }
}

