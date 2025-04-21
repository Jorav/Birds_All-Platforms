using Microsoft.Xna.Framework;
using Birds.src.controllers;
using Birds.src.controllers.steering;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Birds.src.factories
{
    public class ControllerFactory
    {

        public static Controller Create(Vector2 position, ID_CONTROLLER id = ID_CONTROLLER.CONTROLLER_DEFAULT, int numberOfEntities = 1)
        {
            Controller c;
            switch (id)
            {
                case ID_CONTROLLER.CONTROLLER_DEFAULT: return new Controller(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT));
                //case IDs.CHASER_AI: return new ChaserAI(position);
                case ID_CONTROLLER.PLAYER: c = new CohesiveController(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT)); c.Steering = new PlayerSteering(c); return c;
                case ID_CONTROLLER.CHASER_AI: c = new Controller(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT)); c.Steering = new ChaserSteering(c); return c;
                case ID_CONTROLLER.BACKGROUND_SUN: c = new Background(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.SUN, isBackground: true, scale: 4 ), Input.Camera, relativeSpeed: 0.2f); return c;
                case ID_CONTROLLER.FOREGROUND_CLOUD: c = new Background(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.CLOUD, isBackground: true, scale: 3), Input.Camera, relativeSpeed: 1.5f); return c;
                default:
                    throw new NotImplementedException();
            }
        }

        public static String GetName(ID_CONTROLLER id)
        {
            switch (id)
            {
                case ID_CONTROLLER.CONTROLLER_DEFAULT: return Controller.GetName();
                //case IDs.CHASER_AI: return ChaserAI.GetName();
                //case IDs.PLAYER: return Player.GetName();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
