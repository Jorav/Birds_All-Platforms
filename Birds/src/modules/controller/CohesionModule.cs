using Birds.src.controllers;
using Birds.src.entities;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Birds.src.modules.controller
{
  public class CohesionModule
  {
    public Controller controller;
    public static float REPULSIONDISTANCE = 100;

    public CohesionModule(Controller controller)
    {
      this.controller = controller;
    }
    public void Update(GameTime gameTime)
    {
      ApplyInternalGravity();
      ApplyInterParticleGravity();
      ApplyInterParticleRepulsion();
    }

    protected void ApplyInternalGravity()
    {
      Vector2 distanceFromController;
      foreach (IEntity c1 in controller.Entities)
      {
        distanceFromController = controller.Position - c1.Position;
        if (distanceFromController.Length() > c1.Radius)
          c1.Accelerate(Vector2.Normalize(controller.Position - c1.Position), 0.5f * (float)((distanceFromController.Length() - c1.Radius) / AverageDistance()) / c1.Mass);
      }
    }
    private void ApplyInterParticleGravity()
    {
      //throw new NotImplementedException();
    }
    public void ApplyInterParticleRepulsion()
    {
      foreach (IEntity e1 in controller.Entities)
      {
        foreach (IEntity e2 in controller.Entities)//TODO: only allow IsCollidable to affect this?
        {
          if (e1 != e2/* && c1 is Entity e1 && c2 is Entity e2*/)
          {
            if (e1.Radius + e2.Radius + REPULSIONDISTANCE > Vector2.Distance(e1.Position, e2.Position))
            {
              Vector2 vectorToE = e2.Position - e1.Position;
              float distance = vectorToE.Length();
              float res = 0;
              if (distance < 32)
                distance = 32;
              res = 9f / distance;
              vectorToE.Normalize();
              e2.Accelerate(vectorToE, e1.Mass * res);
            }
          }
        }

      }
    }
    protected float AverageDistance()
    {
      float nr = 1;
      float distance = 0;
      float mass = 0;
      foreach (IEntity c in controller.Entities)
      {
        distance += (Vector2.Distance(c.Position, controller.Position) + c.Radius) * c.Mass;
        //nr += 1;
        mass += c.Mass;
      }
      if (mass != 0)
        return distance / nr / mass;
      return 1;
    }

    public virtual object Clone()
    {
      CohesionModule cNew = (CohesionModule)this.MemberwiseClone();
      cNew.controller = controller;
      return cNew;
    }
  }
}
