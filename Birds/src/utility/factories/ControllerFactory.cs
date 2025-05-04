using Microsoft.Xna.Framework;
using Birds.src.controllers;
using Birds.src.controllers.modules.steering;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Birds.src.controllers.modules;

namespace Birds.src.factories
{
    public class ControllerFactory
    {

        public static Controller Create(Vector2 position, ID_CONTROLLER id = ID_CONTROLLER.DEFAULT, int numberOfEntities = 1)
        {
            Controller c;
            switch (id)
            {
                case ID_CONTROLLER.DEFAULT: return new Controller(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT));
                //case IDs.CHASER_AI: return new ChaserAI(position);
                case ID_CONTROLLER.PLAYER: c = new Controller(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT)); c.Steering = new PlayerSteeringModule(c); c.CohesionModule = new CohesionModule(c); return c;
                case ID_CONTROLLER.CHASER_AI: c = new Controller(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.DEFAULT)); c.Steering = new ChaserSteeringModule(c); return c;
                case ID_CONTROLLER.BACKGROUND_SUN: c = new Background(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.SUN, isBackground: true, scale: 4 ), Input.Camera, relativeSpeed: 0.2f); return c;
                case ID_CONTROLLER.FOREGROUND_CLOUD: c = new Background(EntityFactory.CreateEntities(position, numberOfEntities, ID_ENTITY.CLOUD, isBackground: true, scale: 3), Input.Camera, relativeSpeed: 1.5f); return c;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
