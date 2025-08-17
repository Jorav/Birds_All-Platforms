using Birds.src.bounding_areas;
using Birds.src.controllers;
using Birds.src.events;
using Birds.src.modules.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.entities;

public interface IEntity : IModuleContainer
{
  public Controller Manager { get; set; }

  public void Update(GameTime gameTime);
  public void Draw(SpriteBatch sb);
  public void Deprecate();
  public object Clone();
}
