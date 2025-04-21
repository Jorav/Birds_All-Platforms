using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Birds.src.factories;
using Birds.src.entities;

namespace Birds.src.menu
{
    class TestState : GameState
    {
        public TestState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input) : base(game, graphicsDevice, content, input)
        {
            Player.SetEntities(EntityFactory.CreateEntities(Vector2.Zero,10,ID_ENTITY.DEFAULT));
            controller.Add(ControllerFactory.Create(new Vector2(100,100),numberOfEntities: 3, id: ID_CONTROLLER.CONTROLLER_DEFAULT));
            controller.Add(ControllerFactory.Create(new Vector2(200,200),numberOfEntities: 1));
            controller.Add(ControllerFactory.Create(new Vector2(300,300),numberOfEntities: 7));
            controller.Add(ControllerFactory.Create(new Vector2(353,42)));
            foregrounds.Add((Background)ControllerFactory.Create(Vector2.Zero, numberOfEntities:7, id: ID_CONTROLLER.FOREGROUND_CLOUD));
            backgrounds.Add((Background)ControllerFactory.Create(Vector2.Zero, numberOfEntities:1, id: ID_CONTROLLER.BACKGROUND_SUN));
        }
    }
}
