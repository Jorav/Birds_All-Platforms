using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller.steering;
public abstract class SteeringModule : ModuleBase
{
  public bool actionsLocked = false;
  public Vector2 Position { get; set; }
  public abstract bool ShouldAccelerate { get; }
  public virtual bool ShouldRotate { get { return !actionsLocked; } }
  public abstract Vector2 PositionLookedAt { get; }

  protected override void ConfigurePropertySync()
  {
    ReadSync(() => Position, container.Position);
  }

  protected override void Update(GameTime gameTime)
  {
    if (ShouldRotate)
    {
      RotateToTarget();
    }
    if (ShouldAccelerate)
    {
      AccelerateToTarget();
    }
  }

  private void RotateToTarget()
  {
    container.RotateTo(PositionLookedAt);
  }

  private void AccelerateToTarget()
  {
    foreach (IEntity e in container.Entities)
    {
      Vector2 accelerationVector = Vector2.Normalize(PositionLookedAt - e.Position);
      e.Accelerate(accelerationVector);
    }
  }

  public virtual object Clone()
  {
    SteeringModule sNew = (SteeringModule)this.MemberwiseClone();
    return sNew;
  }
}
