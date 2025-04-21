using Birds.src;
using Birds.src.entities;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Birds.src.controllers
{
  public class CohesiveController : Controller
  {
    protected bool integrateSeperatedEntities = false;
    public static float REPULSIONDISTANCE = 100;

    public CohesiveController(List<IEntity> controllables, ID_OTHER team = ID_OTHER.TEAM_AI) : base(controllables, team) { }
    public CohesiveController([OptionalAttribute] Vector2 position, ID_OTHER team = ID_OTHER.TEAM_AI) : base(position, team) { }
    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      ApplyInternalGravity();
      ApplyInterParticleGravity();
      ApplyInterParticleRepulsion();
    }

    protected void ApplyInternalGravity()
    {
      Vector2 distanceFromController;
      foreach (IEntity c1 in entities)
      {
        distanceFromController = Position - c1.Position;
        if (distanceFromController.Length() > c1.Radius)
          c1.Accelerate(Vector2.Normalize(Position - c1.Position), 0.5f * (float)((distanceFromController.Length() - c1.Radius) / AverageDistance()) / c1.Mass);
      }
    }
    private void ApplyInterParticleGravity()
    {
      //throw new NotImplementedException();
    }
    public void ApplyInterParticleRepulsion()
    {
      foreach (IEntity e1 in entities)
      {
        foreach (IEntity e2 in entities)//TODO: only allow IsCollidable to affect this?
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
    /*protected override void AddSeperatedEntities()
    {
        if (integrateSeperatedEntities)
        {
            List<EntityController> seperatedEntities = new List<EntityController>();
            foreach (IControllable c in Controllables)
                if (c is EntityController ec)
                    foreach (EntityController ecSeperated in ec.SeperatedEntities)
                    {
                        if (ecSeperated.Controllables.Count == 1 && !(ecSeperated.Controllables[0] is Composite))
                            ;//((WorldEntity)(ecSeperated.Controllables[0])).Die();
                        else
                            seperatedEntities.Add(ecSeperated);
                    }
            foreach (EntityController ec in seperatedEntities)
            {
                AddControllable(ec);
            }
            foreach (IControllable c in Controllables)
                if (c is EntityController ec)
                    ec.SeperatedEntities.Clear();
        }

        else
            base.AddSeperatedEntities();
    }*/
  }
}
