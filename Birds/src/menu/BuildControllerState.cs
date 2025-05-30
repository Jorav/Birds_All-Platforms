﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Birds.src.utility;
using Birds.src.factories;
using System.Diagnostics;
using Birds.src.entities;

namespace Birds.src.menu
{
    public class BuildControllerState : MenuState
    {
        protected State previousState;
        //public MenuController menuController;
        //protected Controller controllerEdited;
        protected Controller controllerEdited;
        protected Controller originalController;
        protected bool buildMode;
        public int previousScrollValue;
        public int currentScrollValue;
        private readonly Sprite overlay;
        protected Color originalColor;
        private bool wasPressed = true;
        private bool playerLastClicked = false;
        Stopwatch timer = new Stopwatch();
        private int doubleClickTreshold = 400;
        public BuildControllerState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, State previousState, Input input, Controller controllerEdited/*, MenuController menuController = null*/) : base(game, graphicsDevice, content, input)
        {
            this.controllerEdited = (Controller)controllerEdited.Clone();
            originalController = controllerEdited;

            Input.Camera.Controller = this.controllerEdited;
            this.controllerEdited.Steering.actionsLocked = true;
            this.controllerEdited.Rotation = 0;
            /*
            if (menuController == null)
                this.menuController = new MenuController(CopyEntitiesFromController(controllerEdited), input);
            else
                this.menuController = menuController;
                
            this.menuController.Camera.InBuildScreen = true;
            this.menuController.Camera.AutoAdjustZoom = true;
            
            this.menuController.Color = Color.White;
                */
            this.previousState = previousState;
            if (previousState is GameState)
                GameState.Player.Steering.actionsLocked = true;
            //currentScrollValue = input.ScrollValue;
            overlay = SpriteFactory.GetSprite(ID_SPRITE.BACKGROUND_WHITE, new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2), SpriteFactory.textures[(int)ID_SPRITE.BACKGROUND_WHITE].Height / Game1.ScreenHeight);
            originalColor = controllerEdited.Color;
            components = new();
        }

        /*protected List<IEntity> CopyEntitiesFromController(Controller controller)
        {
            List<IEntity> collidables = new List<IEntity>();
            foreach (IEntity c in controller.Controllables)
                collidables.Add((IEntity)c.Clone());
            return collidables;
        }*/

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            previousScrollValue = currentScrollValue;
            controllerEdited.Update(gameTime);
            HandleScroll();
            CheckDoubleClick();
            wasPressed = Input.IsPressed;
        }
        private void HandleScroll()
        {
            Input.HandleZoom();
        }
        private void CheckDoubleClick()
        {
            if (timer.IsRunning && timer.ElapsedMilliseconds >= doubleClickTreshold)
            {
                timer.Stop();
                timer.Reset();
            }
            if (!wasPressed && Input.IsPressed)
            {
                if (timer.IsRunning)
                {
                    if (playerLastClicked)
                    {
                        if (controllerEdited.BoundingCircle.Contains(Input.PositionGameCoords))
                        {

                            controllerEdited.AddControllable(EntityFactory.GetEntity(controllerEdited.Position, ID_ENTITY.DEFAULT));
                            ;//TODO: CheckCollisionWithEntitiesInControllerEdited();
                        }
                        else
                        {
                            timer.Restart();
                            playerLastClicked = false;
                        }
                    }
                    else
                    {
                        if (controllerEdited.BoundingCircle.Contains(Input.PositionGameCoords))
                        {
                            timer.Restart();
                            playerLastClicked = true;
                        }
                        else
                        {
                            ReturnToPreviousState();
                        }
                    }
                }
                else
                {
                    if (controllerEdited.BoundingCircle.Contains(Input.PositionGameCoords))
                    {
                        playerLastClicked = true;
                    }
                    else
                        playerLastClicked = false;
                    timer.Reset();
                    timer.Start();
                }
            }
        }

        private void ReturnToPreviousState()
        {
            game.ChangeState(previousState);
            foreach (IEntity e in originalController.Entities)
            {
                e.Deprecate();
            }
            originalController.SetEntities(controllerEdited.Entities);
            originalController.Steering.actionsLocked = false;
            Input.Camera.Controller = originalController;
        }

        private void HandleDoubleClick()
        {
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            previousState.Draw(gameTime, spriteBatch);
            spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.AnisotropicClamp);
            overlay.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(transformMatrix: Input.Camera.Transform, sortMode: SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, samplerState: SamplerState.AnisotropicClamp);
            controllerEdited.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime, spriteBatch);
        }
    }
}
