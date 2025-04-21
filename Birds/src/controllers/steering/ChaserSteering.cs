using System;
using Birds.src.controllers.steering;
using Birds.src.menu;
using Birds.src.utility;
using Microsoft.Xna.Framework;

namespace Birds.src.controllers.steering
{
  public class ChaserSteering : Steering
  {
    public ICollidable target;
    public override bool ShouldRotate
    {
      get
      {
        if (target != null && !actionsLocked)
          return base.ShouldRotate;
        else
          return false;
      }
    }
    public bool FocusPlayer { get; set; } = true;

    public ChaserSteering(Controller controller) : base(controller)
    {
    }

    public override bool ShouldAccelerate
    {
      get
      {
        return target != null && !actionsLocked;
      }

    }

    public override Vector2 PositionLookedAt
    {
      get
      {
        return target.Position;
      }
    }

    private void UpdateTarget()
    {
      float shortestDistance = Vector2.Distance(GameState.Player.Position, controller.Position);
      Controller bestController = GameState.Player;
      foreach (Controller c in GameController.controllers)
      {
        float distanceTemp = Vector2.Distance(c.Position, controller.Position);
        if (distanceTemp < shortestDistance)
        {
          if (FocusPlayer && c.Steering is PlayerSteering)
          {
            shortestDistance = distanceTemp;
            bestController = c;
          }
          else if (!FocusPlayer)
          {
            shortestDistance = distanceTemp;
            bestController = c;
          }
        }
      }
      target = bestController;
    }
    public override void Update(GameTime gameTime)
    {
      UpdateTarget();
      base.Update(gameTime);
    }
    public override object Clone()
    {
      ChaserSteering steeringNew = (ChaserSteering)base.Clone();
      steeringNew.target = target;
      return steeringNew;
    }
  }
}