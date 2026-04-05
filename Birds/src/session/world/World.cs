using Birds.src.collision;
using Birds.src.collision.BVH;
using Birds.src.containers.controller;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.modules.shared.collision_detection;
using Birds.src.player;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.session.world;

public class World
{
  private readonly List<Controller> playerControllers = new();
  private readonly List<Controller> otherControllers = new();
  private readonly AABBTree collisionManager = new();

  public List<Background> Backgrounds { get; } = new();
  public List<Background> Foregrounds { get; } = new();
  public IEnumerable<Controller> Controllers => playerControllers.Concat(otherControllers);

  public void AddController(Controller c) => otherControllers.Add(c);
  public void RemoveController(Controller c) => otherControllers.Remove(c);

  public Controller AddPlayer(IInputState input)
  {
    var playerController = ControllerFactory.Create(
        CompositeControllerFactory.CreateComposites(Vector2.Zero, 1, CompositeControllerFactory.DEFAULT_SINGLE),
        ID_CONTROLLER.PLAYER,
        input
    );
    playerControllers.Add(playerController);
    return playerController;
  }

  public void RemovePlayer(Controller c) => playerControllers.Remove(c);

  public void Update(GameTime gameTime)
  {
    foreach (var c in Controllers) c.Update(gameTime);
    UpdateCollisions();
    foreach (var b in Backgrounds) b.Update(gameTime);
    foreach (var f in Foregrounds) f.Update(gameTime);
  }

  private void UpdateCollisions()
  {
    var collidables = Controllers
        .Select(c => c.GetModule<GroupCollisionDetectionModule>())
        .Where(m => m != null)
        .Cast<ICollidable>()
        .ToList();

    if (collidables.Count > 0)
    {
      collisionManager.BuildTree(collidables);
      collisionManager.AddInternalCollisions();
    }
  }
}
