
using Birds.src.entities;
using Microsoft.Xna.Framework;

namespace Birds.src.controllers.modules.steering
{
    public abstract class SteeringModule
    {
        public bool actionsLocked = false;
        public Controller controller;
        public abstract bool ShouldAccelerate { get; }
        public virtual bool ShouldRotate { get { return !actionsLocked; } }
        public abstract Vector2 PositionLookedAt { get; }
        private bool moveWholeController = false;

        public SteeringModule(Controller controller)
        {
            this.controller = controller;
        }
        public virtual void Update(GameTime gameTime)
        {
            if (ShouldRotate)
                controller.RotateTo(PositionLookedAt);
            if (ShouldAccelerate)
            {
                if (moveWholeController)
                {
                    Vector2 accelerationVector = Vector2.Normalize(PositionLookedAt - controller.Position);
                    controller.Accelerate(accelerationVector);
                }

                else
                {
                    foreach (IEntity e in controller.Entities)
                    {
                        Vector2 accelerationVector = Vector2.Normalize(PositionLookedAt - e.Position);
                        e.Accelerate(accelerationVector);
                    }
                }
            }
        }
        public virtual object Clone()
        {
            SteeringModule sNew  = (SteeringModule)this.MemberwiseClone();
            sNew.controller = controller;
            return sNew;
        }
    }
}