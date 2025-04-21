using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src
{
    public interface ICollidable
    {
        public Vector2 Position { get;}
        public float Radius { get; }
        public float Mass {get;}
        public IBoundingArea BoundingArea {get;}
        public bool IsCollidable{get;}
        public bool CollidesWith(ICollidable otherEntity);
        public void Collide(ICollidable otherEntity);
    }
}