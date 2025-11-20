using Birds.src.containers.controller;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.utility;

namespace Birds.src.containers.entity;
public class WorldEntity : ModuleContainer, IEntity
{
  public bool IsFiller { get; set; }
  public ID_ENTITY EntityID { get; set; }
  public Controller Manager { get; set; }

  public WorldEntity()
  {
  }

  public virtual object Clone()
  {
    var cloned = (WorldEntity)base.Clone();
    cloned.IsFiller = this.IsFiller;
    cloned.EntityID = this.EntityID;
    cloned.Manager = null;
    return cloned;
  }
  public void Dispose()
  {
    EntityFactory.availableEntities.Push(this);
  }
}

