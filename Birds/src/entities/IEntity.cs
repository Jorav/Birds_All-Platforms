using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.modules.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.entities
{
  public interface IEntity : ICollidable
  {
    public EntityMovementModule MovementModule { get; }
    public new Vector2 Position //implement in subclasses
    {
      get => MovementModule.Position;
      set => MovementModule.Position = value;
    }
    public new float Rotation { get; set; } //implement in subclasses
    float ICollidable.Mass => MovementModule.Mass;
    float Radius { get; }
    public Controller Manager { get; set; }
    public Vector2 Velocity => MovementModule.Velocity;
    public BoundingCircle BoundingCircle { get; }
    public void AccelerateTo(Vector2 position, float thrust)
    {
      MovementModule.AccelerateTo(position, thrust);
    }
    void Accelerate(Vector2 directionalVector, float thrust)
    {
      MovementModule.Accelerate(directionalVector, thrust);
    }
    void Accelerate(Vector2 directionalVector)
    {
      MovementModule.Accelerate(directionalVector);
    }
    void RotateTo(Vector2 position)
    {
      MovementModule.RotateTo(position);
    }
    public void Update(GameTime gameTime);
    public void Draw(SpriteBatch sb);
    public void Deprecate();
    public object Clone();
  }
}