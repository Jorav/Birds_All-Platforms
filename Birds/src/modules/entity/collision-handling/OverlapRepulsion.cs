using Birds.src.events;
using Birds.src.modules.shared.collision_detection;

namespace Birds.src.modules.entity.collision_handling;

public class OverlapRepulsion : CollisionResponse
{
  public override void HandleCollision(IModuleContainer self, IModuleContainer other)
  {
    if (self.GetManager() != null)
    {
      return;
    }

    var movementModule = self.GetModule<MovementModule>();
    var otherMovementModule = other.GetModule<MovementModule>();
    var bcCollisionDetectionModule = self.GetModule<BaseCollisionDetectionModule>();
    var otherBcCollisionDetectionModule = other.GetModule<BaseCollisionDetectionModule>();

    if (movementModule == null || otherMovementModule == null)
    {
      return;
    }

    var multiplier = other.GetManager() == null ? (other.Entities.Count > 0 ? 0.3f : 1f) : 0.7f;

    movementModule.TotalExteriorForce += bcCollisionDetectionModule.BoundingCircle.CalculateOverlapRepulsion(
      otherBcCollisionDetectionModule.BoundingCircle) * multiplier;
  }
}