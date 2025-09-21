using Birds.src.utility;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller.steering;

public class PlayerSteeringModule : SteeringModule
{
  private bool hasStartedMoving;
  private bool wasPressed;

  public PlayerSteeringModule() : base()
  {
  }

  public override bool ShouldAccelerate
  {
    get
    {
      bool accelerate = false;
      if (actionsLocked)
      {
        return accelerate;
      }
      if (Input.IsPressed && !wasPressed)
      {
        hasStartedMoving = true;
      }
      if (Input.IsPressed && hasStartedMoving)
      {
        accelerate = true;
      }
      else
      {
        hasStartedMoving = false;
      }
      wasPressed = Input.IsPressed;
      return accelerate;
    }

  }

  public override Vector2 PositionLookedAt
  {
    get
    {
      return Input.PositionGameCoords;
    }
  }

  //ADD: When updating, get the state from the player via Input
}