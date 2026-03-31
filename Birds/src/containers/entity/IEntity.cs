using Birds.src.events;
using Birds.src.modules.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;

namespace Birds.src.containers.entity;

public interface IEntity : IModuleContainer
{
  public ID_ENTITY EntityID { get; set; }
  public void Update(GameTime gameTime);
  public bool ReplaceEntity(IEntity oldEntity, IEntity newEntity)
  {
    newEntity.Position.Value = oldEntity.Position.Value;
    newEntity.Rotation.Value = oldEntity.Rotation.Value;

    Entities.Remove(oldEntity);
    foreach (IEntity e in Entities)
    {
      if (e == oldEntity || e.EntityID == ID_ENTITY.FILLER)
        continue;

      if (e.CollidesWith(newEntity))
      {
        Entities.Add(oldEntity);
        return false;
      }
    }
    Entities.Add(newEntity);
    return true;
  }

  public bool ReplaceAndAttach(IEntity oldEntity, IEntity newEntity)
  {
    var oldLinkModule = oldEntity.GetModule<LinkModule>();
    if (oldLinkModule == null)
    {
      return ReplaceEntity(oldEntity, newEntity);
    }

    return oldLinkModule.ReplaceWithNewEntity(newEntity, this);
  }
}
