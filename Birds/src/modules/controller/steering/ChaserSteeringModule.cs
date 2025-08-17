using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.menu;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller.steering
{
  public class ChaserSteeringModule : SteeringModule
  {
    public IEntity target;
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

    public ChaserSteeringModule() : base()
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
      float shortestDistance = Vector2.Distance(GameState.Player.Position, Position);
      Controller bestController = GameState.Player;
      foreach (Controller c in GameController.controllers)
      {
        float distanceTemp = Vector2.Distance(c.Position, Position);
        if (distanceTemp >= shortestDistance)
        {
          continue;
        }
        if (FocusPlayer && c.GetModule<SteeringModule>() is PlayerSteeringModule)
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
      target = bestController.Entities[0];
    }

    protected override void Update(GameTime gameTime)
    {
      UpdateTarget();
      base.Update(gameTime);
    }

    public override object Clone()
    {
      ChaserSteeringModule steeringNew = (ChaserSteeringModule)base.Clone();
      steeringNew.target = target;
      return steeringNew;
    }
  }
}