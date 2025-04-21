using Birds.src.controllers.steering;
using Birds.src.utility;
using Microsoft.Xna.Framework;

namespace Birds.src.controllers.steering
{
  public class PlayerSteering : Steering
  {
    private bool hasStartedMoving;
    private bool wasPressed;

    public PlayerSteering(Controller controller) : base(controller)
    {
    }

    public override bool ShouldAccelerate
    {
      get
      {
        bool accelerate = false;
        //Vector2 accelerationVector = Vector2.Zero;

        if (!actionsLocked)
        {
          if (Input.IsPressed && !wasPressed/* && !controller.BoundingCircle.Contains(Input.PositionGameCoords)*/)
            hasStartedMoving = true;
          if (Input.IsPressed && hasStartedMoving)
          {
            //accelerationVector = Vector2.Normalize(Input.PositionGameCoords - controller.Position);
            //controller.Accelerate(accelerationVector);
            accelerate = true;
          }
          else
            hasStartedMoving = false;
          wasPressed = Input.IsPressed;
        }
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
}