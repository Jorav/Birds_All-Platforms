using Birds.src.events;

namespace Birds.src.modules.entity.collision_handling;

public abstract class CollisionResponse
{
  public abstract void HandleCollision(IModuleContainer self, IModuleContainer other);
  public object Clone()
  {
    return this.MemberwiseClone();
  }
}

