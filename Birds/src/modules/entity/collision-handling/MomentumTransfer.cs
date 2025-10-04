using Birds.src.collision;
using Birds.src.events;

namespace Birds.src.modules.entity.collision_handling;
public class MomentumTransfer : CollisionResponse
{
  public override void HandleCollision(IModuleContainer self, IModuleContainer other)
  {
    var movementModule = self.GetModule<MovementModule>();
    var otherMovementModule = other.GetModule<MovementModule>();

    if (movementModule == null || otherMovementModule == null)
    {
      return;
    }

    movementModule.TotalExteriorForce += movementModule.CalculateCollissionRepulsion(otherMovementModule);
  }
}