using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.entities
{
    public interface IEntity : ICollidable
    {
        public new Vector2 Position {get; set;}
        public new float Rotation {get; set;}
        public Controller Manager {get; set;}
        public Vector2 Velocity { get; }
        public void AccelerateTo(Vector2 position, float thrust);
        public void Deprecate();
    }
}