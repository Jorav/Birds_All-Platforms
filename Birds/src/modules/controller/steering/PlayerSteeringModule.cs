using Birds.src.player;
using Microsoft.Xna.Framework;

namespace Birds.src.modules.controller.steering;

public class PlayerSteeringModule(IInputState playerInput) : SteeringModule()
{
  private bool hasStartedMoving;
  private bool wasPressed;

  public override bool ShouldAccelerate
  {
    get
    {
      bool accelerate = false;
      if (actionsLocked)
      {
        return accelerate;
      }
      if (playerInput.IsPressed && !wasPressed)
      {
        hasStartedMoving = true;
      }
      if (playerInput.IsPressed && hasStartedMoving)
      {
        accelerate = true;
      }
      else
      {
        hasStartedMoving = false;
      }
      wasPressed = playerInput.IsPressed;
      return accelerate;
    }
  }

  public override Vector2 PositionLookedAt
  {
    get
    {
      return playerInput.PositionGameCoords;
    }
  }
}
