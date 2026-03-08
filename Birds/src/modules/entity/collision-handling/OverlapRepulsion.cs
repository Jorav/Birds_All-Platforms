using Birds.src.collision.bounding_areas;
using Birds.src.events;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity.collision_handling;

public class OverlapRepulsion : CollisionResponse
{
  private static readonly Random random = new Random();

  public override void HandleCollision(IModuleContainer self, IModuleContainer other)
  {
    if (self.GetManager() != null)
    {
      return;
    }

    var movementModule = self.GetModule<MovementModule>();
    var bcCollisionDetectionModule = self.GetModule<BaseCollisionDetectionModule>();
    var otherBcCollisionDetectionModule = other.GetModule<BaseCollisionDetectionModule>();

    if (movementModule == null)
    {
      return;
    }

    var multiplier = other.GetManager() == null
      ? (other.Entities.Count > 0 ? 0.1f : 1f) 
      : 0.9f;

    movementModule.TotalExteriorForce += CalculateOverlapRepulsion(
      bcCollisionDetectionModule.BoundingCircle,
      otherBcCollisionDetectionModule.BoundingCircle,
      self.Mass,
      other.Mass) * multiplier;
  }

  public Vector2 CalculateOverlapRepulsion(BoundingCircle self, BoundingCircle other, float massSelf, float massOther)
  {
    Vector2 distanceVector = self.Position - other.Position;
    float distance = distanceVector.Length();

    if (distance < 1f)
    {
      float angle = (float)(random.NextDouble() * Math.PI * 2);
      distanceVector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
      distance = 1f;
    }

    float overlap = self.Radius + other.Radius - distance;
    if (overlap <= 0)
      return Vector2.Zero;

    Vector2 direction = distanceVector / distance;

    float maxJitterDegrees = 15f;
    float jitterRad = (float)((random.NextDouble() * 2 - 1) * maxJitterDegrees * Math.PI / 180.0);
    float cos = (float)Math.Cos(jitterRad);
    float sin = (float)Math.Sin(jitterRad);
    direction = new Vector2(direction.X * cos - direction.Y * sin, direction.X * sin + direction.Y * cos);

    float linearOverlap = Math.Min(overlap, 16f);
    float linearForce = linearOverlap * 50f;

    float quadraticForce = 0f;
    if (overlap > 16f)
    {
      float excessOverlap = overlap - 16f;
      quadraticForce = excessOverlap * excessOverlap * 0.4f;
    }

    float totalForce = (linearForce + quadraticForce / (massSelf + massOther)) * 0.001f;

    return direction * totalForce;
  }
}