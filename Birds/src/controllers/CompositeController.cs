using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.controllers.steering;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.entities
{
    public class CompositeController : Controller, IController, IEntity
    {
        public float Rotation { get { return rotation; }
            set
            {
                float dRotation = Rotation - value;
                foreach (WorldEntity e in entities)
                {
                    Vector2 relativePosition = e.Position - Position;
                    Vector2 newRelativePosition = Vector2.Transform(relativePosition, Matrix.CreateRotationZ(-dRotation));
                    e.Velocity = newRelativePosition - relativePosition;
                    e.Rotation = Rotation;

                }
                rotation = value;
            }
        }
        private float rotation;

        public Controller Manager { get; set; }

        public Vector2 Velocity => throw new NotImplementedException();

        public void AddControllable(IEntity c)
        {
            throw new NotImplementedException();
        }

        public void SetEntities(List<IEntity> newEntities)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Collide(ICollidable otherEntity)
        {
            throw new NotImplementedException();
        }

        public bool CollidesWith(ICollidable otherEntity)
        {
            throw new NotImplementedException();
        }

        public void Deprecate()
        {
            throw new NotImplementedException();
        }

        public override void RotateTo(Vector2 position)
        {
            Rotation = Movable.CalculateRotation(position, Position);
        }
    }
}
