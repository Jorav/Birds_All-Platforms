using Microsoft.Xna.Framework;
using Birds.src.utility;
using System;
using Birds.src.modules.controller.steering;
using Birds.src.modules.controller;
using Birds.src.containers.controller;
using System.Collections.Generic;
using Birds.src.containers.entity;
using Birds.src.collision.BVH;
using Birds.src.modules.shared.collision_detection;

namespace Birds.src.factories
{
  public class ControllerFactory
  {
    public static Controller Create(List<IEntity> entities, ID_CONTROLLER id = ID_CONTROLLER.DEFAULT)
    {
      Controller c;
      switch (id)
      {
        case ID_CONTROLLER.DEFAULT:
          c = new Controller(entities);
          c.AddModule(new GroupCollisionClearer());
          c.AddModule(new GroupMassModule());
          c.AddModule(new GroupWeightedPositionModule());
          c.AddModule(new GroupMovementModule());
          c.AddModule(new GroupRotationModule());
          c.AddModule(new GroupRadiusModule());
          c.AddModule(new GroupCollisionDetectionModule(
              new AABBTree(),
              evaluateInternalCollisions: true
          ));
          c.AddModule(new GroupDrawModule());
          return c;

        case ID_CONTROLLER.PLAYER:
          c = new Controller(entities);
          c.AddModule(new GroupCollisionClearer());
          c.AddModule(new GroupMassModule());
          c.AddModule(new GroupWeightedPositionModule());
          c.AddModule(new GroupMovementModule());
          c.AddModule(new GroupRotationModule());
          c.AddModule(new GroupRadiusModule());
          c.AddModule(new PlayerSteeringModule());
          c.AddModule(new CohesionModule());
          c.AddModule(new GroupCollisionDetectionModule(
              new AABBTree(),
              evaluateInternalCollisions: true
          ));
          c.AddModule(new GroupDrawModule());
          return c;

        case ID_CONTROLLER.CHASER_AI:
          c = new Controller(entities);
          c.AddModule(new GroupCollisionClearer());
          c.AddModule(new GroupMassModule());
          c.AddModule(new GroupWeightedPositionModule());
          c.AddModule(new GroupMovementModule());
          c.AddModule(new GroupRotationModule());
          c.AddModule(new GroupRadiusModule());
          c.AddModule(new ChaserSteeringModule());
          c.AddModule(new GroupCollisionDetectionModule(
              new AABBTree(),
              evaluateInternalCollisions: true
          ));
          c.AddModule(new GroupDrawModule());
          return c;

        case ID_CONTROLLER.BACKGROUND_SUN:
          c = new Background(entities, Input.Camera, relativeSpeed: 0.2f);//scale used to be 4
          c.AddModule(new GroupDrawModule());
          c.AddModule(new GroupRadiusModule());
          return c;

        case ID_CONTROLLER.FOREGROUND_CLOUD:
          c = new Background(entities, Input.Camera, relativeSpeed: 1.5f);//scale used to be 3
          c.AddModule(new GroupDrawModule());
          c.AddModule(new GroupRadiusModule());
          return c;

        default:
          throw new NotImplementedException();
      }
    }


    public static Controller Create(Vector2 position, ID_CONTROLLER id = ID_CONTROLLER.DEFAULT, int numberOfEntities = 1)
    {
      Controller c;
      switch (id)
      {
        case ID_CONTROLLER.DEFAULT:
          c = new Controller(WorldEntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT));
          c.AddModule(new GroupCollisionClearer());
          c.AddModule(new GroupMassModule());
          c.AddModule(new GroupWeightedPositionModule());
          c.AddModule(new GroupMovementModule());
          c.AddModule(new GroupRotationModule());
          c.AddModule(new GroupRadiusModule());
          c.AddModule(new GroupCollisionDetectionModule(
              new AABBTree(),
              evaluateInternalCollisions: true
          ));
          c.AddModule(new GroupDrawModule());
          c.Position.Value = position;
          return c;

        case ID_CONTROLLER.PLAYER:
          c = new Controller(WorldEntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT));
          c.AddModule(new GroupCollisionClearer());
          c.AddModule(new GroupMassModule());
          c.AddModule(new GroupWeightedPositionModule());
          c.AddModule(new GroupMovementModule());
          c.AddModule(new GroupRotationModule());
          c.AddModule(new GroupRadiusModule());
          c.AddModule(new PlayerSteeringModule());
          c.AddModule(new CohesionModule());
          c.AddModule(new GroupCollisionDetectionModule(
              new AABBTree(),
              evaluateInternalCollisions: true
          ));
          c.AddModule(new GroupDrawModule());
          c.Position.Value = position;
          return c;

        case ID_CONTROLLER.CHASER_AI:
          c = new Controller(WorldEntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT));
          c.AddModule(new GroupCollisionClearer());
          c.AddModule(new GroupMassModule());
          c.AddModule(new GroupWeightedPositionModule());
          c.AddModule(new GroupMovementModule());
          c.AddModule(new GroupRotationModule());
          c.AddModule(new GroupRadiusModule());
          c.AddModule(new ChaserSteeringModule());
          c.AddModule(new GroupCollisionDetectionModule(
              new AABBTree(),
              evaluateInternalCollisions: true
          ));
          c.AddModule(new GroupDrawModule());
          c.Position.Value = position;
          return c;

        case ID_CONTROLLER.BACKGROUND_SUN:
          c = new Background(WorldEntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.SUN, isBackground: true), Input.Camera, relativeSpeed: 0.2f);//scale used to be 4
          c.AddModule(new GroupDrawModule());
          c.AddModule(new GroupRadiusModule());
          c.Position.Value = position;
          return c;

        case ID_CONTROLLER.FOREGROUND_CLOUD:
          c = new Background(WorldEntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.CLOUD, isBackground: true), Input.Camera, relativeSpeed: 1.5f);//scale used to be 3
          c.AddModule(new GroupDrawModule());
          c.AddModule(new GroupRadiusModule());
          c.Position.Value = position;
          return c;

        default:
          throw new NotImplementedException();
      }
    }
  }
}
