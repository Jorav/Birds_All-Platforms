using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src;
  public interface ICollidable
  {
      public Vector2 Position { get;}
      public float Radius { get; }
      public float Mass {get; }

      //NEW ONES
      public float Rotation { set; }
      public Color Color { get; set; }
      public ID_OTHER Team { get; set; }
      public void Update(GameTime gameTime);
      public void Draw(SpriteBatch sb);
      public void Accelerate(Vector2 directionalVector, float thrust);
      public void Accelerate(Vector2 directionalVector);
      public void RotateTo(Vector2 position);
      public object Clone();
      public BoundingCircle BoundingCircle { get; }
      //NEW ONES

      public IBoundingArea BoundingArea {get;}
      public bool IsCollidable{get;}
      public bool CollidesWith(ICollidable otherCollidable);
      public void Collide(ICollidable otherEntity);
  }