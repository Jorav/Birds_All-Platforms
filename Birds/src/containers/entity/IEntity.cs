using Birds.src.containers.controller;
using Birds.src.events;
using Microsoft.Xna.Framework;

namespace Birds.src.containers.entity;

public interface IEntity : IModuleContainer
{
  public Controller Manager { get; set; }
  public void Update(GameTime gameTime);
  public void Dispose();
  public bool ReplaceEntity(IEntity oldEntity, IEntity newEntity)
  {
    newEntity.Position.Value = oldEntity.Position;
    newEntity.Rotation.Value = oldEntity.Rotation;
    Entities.Remove(oldEntity);
    foreach(IEntity e in Entities)
    {
      if (e.CollidesWith(newEntity))
      {
        Entities.Add(oldEntity);
        return false;
      }
    }
    Entities.Add(newEntity);
    return true;
  }
}
