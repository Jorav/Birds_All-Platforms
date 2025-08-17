using Microsoft.Xna.Framework;
using Birds.src.controllers;
using Birds.src.utility;
using System;
using Birds.src.modules.controller.steering;
using Birds.src.modules.controller;
using Birds.src.modules.entity;
using Birds.src.modules.shared.bounding_area;
using Birds.src.modules.collision;

namespace Birds.src.factories
{
  public class ControllerFactory
  {
    public static Controller Create(Vector2 position, ID_CONTROLLER id = ID_CONTROLLER.DEFAULT, int numberOfEntities = 1)
    {
      Controller c;
      switch (id)
      {
        case ID_CONTROLLER.DEFAULT:
          c = new Controller(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT));
          c.AddModule(new ControllerMovementModule());
          c.AddModule(new BCCollisionDetectionModule());
          c.AddModule(new ControllerCollisionHandlerModule());
          return c;

        case ID_CONTROLLER.PLAYER:
          c = new Controller(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT));
          c.AddModule(new ControllerMovementModule());
          c.AddModule(new BCCollisionDetectionModule());
          c.AddModule(new ControllerCollisionHandlerModule());
          c.AddModule(new PlayerSteeringModule());
          c.AddModule(new CohesionModule());
          return c;

        case ID_CONTROLLER.CHASER_AI:
          c = new Controller(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT));
          c.AddModule(new ControllerMovementModule());
          c.AddModule(new BCCollisionDetectionModule());
          c.AddModule(new ControllerCollisionHandlerModule());
          c.AddModule(new ChaserSteeringModule());
          return c;

        case ID_CONTROLLER.BACKGROUND_SUN:
          c = new Background(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.SUN, isBackground: true), Input.Camera, relativeSpeed: 0.2f);//scale used to be 4
          return c;

        case ID_CONTROLLER.FOREGROUND_CLOUD:
          c = new Background(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.CLOUD, isBackground: true), Input.Camera, relativeSpeed: 1.5f);//scale used to be 3
          return c;

        default:
          throw new NotImplementedException();
      }
    }
  }
}
