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
    public MovementModule MovementModule { get; }
    public new Vector2 Position //implement in subclasses
    {
      get => MovementModule.Position;
      set => MovementModule.Position = value;
    }
    public new float Rotation { get; set; } //implement in subclasses
    float ICollidable.Mass => MovementModule.Mass;
    public Controller Manager { get; set; }
    public Vector2 Velocity => MovementModule.Velocity;
    public void AccelerateTo(Vector2 position, float thrust)
    {
      MovementModule.AccelerateTo(position, thrust);
    }
    void ICollidable.Accelerate(Vector2 directionalVector, float thrust)
    {
      MovementModule.Accelerate(directionalVector, thrust);
    }
    void ICollidable.Accelerate(Vector2 directionalVector)
    {
      MovementModule.Accelerate(directionalVector);
    }
    void ICollidable.RotateTo(Vector2 position)
    {
      MovementModule.RotateTo(position);
    }
    public void Deprecate();
  }
}