using Birds.src.collision;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.shared.bounding_area;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity.collision_handling;
public class OverlapRepulsion : CollisionResponse
{
  public override void HandleCollision(IModuleContainer self, IModuleContainer other)
  {
    var movementModule = self.GetModule<MovementModule>();
    var otherMovementModule = other.GetModule<MovementModule>();
    var bcCollisionDetectionModule = self.GetModule<BCCollisionDetectionModule>();
    var otherBcCollisionDetectionModule = other.GetModule<BCCollisionDetectionModule>();

    if (movementModule == null || otherMovementModule == null)
    {
      return;
    }

    movementModule.TotalExteriorForce += bcCollisionDetectionModule.BoundingCircle.CalculateOverlapRepulsion(
      otherBcCollisionDetectionModule.BoundingCircle);
  }
}