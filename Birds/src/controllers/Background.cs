using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src;
using Birds.src.controllers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Birds.src.utility;
using Birds.src.entities;

namespace Birds.src.controllers
{
    public class Background : Controller
    {
        protected float relativeSpeed;
        protected Camera camera;
        protected Vector2 movement;
        float previousZoom;
        public Background(List<IEntity> entities, Camera camera, [OptionalAttribute] Vector2 movement, float relativeSpeed = 1) : base(entities)
        {
            this.relativeSpeed = relativeSpeed;
            this.camera = camera;
            if (movement == null)
                movement = Vector2.Zero;
            this.movement = movement;
            previousZoom = camera.Zoom;
        }
        
        public override void Update(GameTime gameTime) //OBS: assumes background sprites not rotated
        {
            foreach (WorldEntity e in Entities)
            {
                Vector2 cameraChange = camera.Position - camera.PreviousPosition;
                Vector2 positionChange = cameraChange* (1 - relativeSpeed) + movement*relativeSpeed;
                //e.Accelerate(movement * relativeSpeed);
                e.Position += positionChange;
                e.Scale += (camera.Zoom-previousZoom)*relativeSpeed; 
                
                //e.Scale = 1.5f-camera.Zoom*(relativeSpeed);
                e.TotalExteriorForce *= (1-relativeSpeed);
                e.Update(gameTime);
                UpdatePosition();
                UpdateRadius();
            }previousZoom = camera.Zoom;
            /*
            foreach (WorldEntity e in Entities)
            {
                float positionX = e.Position.X;
                float positionY = e.Position.Y;
                if (positionX + e.Width*e.Scale*camera.Zoom <= camera.Position.X-camera.Width/2)
                    positionX += camera.Width + e.Width*e.Scale * camera.Zoom;//change this to setting new position instead of addition
                else if(positionX-e.Width*e.Scale * camera.Zoom > camera.Position.X+camera.Width/2)
                    positionX -= camera.Width+e.Width * camera.Zoom;
                if (positionY + e.Height*e.Scale * camera.Zoom <= camera.Position.Y-camera.Height/2)
                    positionY += camera.Height + e.Height * camera.Zoom;
                else if (positionY - e.Height*e.Scale * camera.Zoom > camera.Position.Y + camera.Height/2)
                    positionY -= camera.Height+e.Height * camera.Zoom;
                if (positionX != e.Position.X || positionY != e.Position.Y)
                    e.Position = new Vector2(positionX, positionY);
            }*/
        }
    }
}