using Birds.src.containers.controller;
using Birds.src.factories;
using Birds.src.utility;
using Microsoft.Xna.Framework;

namespace Birds.src.session.world;
public static class WorldInitializer
{
  public static void Initialize(World world)
  {
    world.AddController(ControllerFactory.Create(
        new Vector2(100, 100), numberOfEntities: 10, id: ID_CONTROLLER.DEFAULT)
    );

    world.Foregrounds.Add((Background)ControllerFactory.Create(
        Vector2.Zero, numberOfEntities: 14, id: ID_CONTROLLER.FOREGROUND_CLOUD)
    );
    world.Backgrounds.Add((Background)ControllerFactory.Create(
        Vector2.Zero, numberOfEntities: 1, id: ID_CONTROLLER.BACKGROUND_SUN)
    );
  }
}
