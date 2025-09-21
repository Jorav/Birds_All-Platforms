using Birds.src.containers.controller;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.containers.entity;

public interface IEntity : IModuleContainer
{
  public Controller Manager { get; set; }

  public void Update(GameTime gameTime);
  public void Deprecate();
  public object Clone();
}
