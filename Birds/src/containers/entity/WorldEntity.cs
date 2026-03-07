using Birds.src.containers.controller;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.utility;

namespace Birds.src.containers.entity;
public class WorldEntity : ModuleContainer, IEntity
{
  public bool IsFiller { get; set; }
  public ID_ENTITY EntityID { get; set; }

  public WorldEntity()
  {
  }

  public virtual object Clone()
  {
    var cloned = (WorldEntity)base.Clone();
    return cloned;
  }

  public override void Dispose()
  {
    base.Dispose();
    WorldEntityFactory.availableEntities.Push(this);
  }
}

