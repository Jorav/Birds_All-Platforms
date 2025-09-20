using Birds.src.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.entity;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Birds.src.containers.composite;

public class CompositeController : ModuleContainer, IController, IEntity //remove Controller
{
  public Controller Manager { get; set; }

  public CompositeController() : base()
  {
    //CollisionManager.ResolveInternalCollisions = false;
  }
  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
  }

  public void AddEntity(IEntity e)
  {
    //bool collidesWithSubentities = CollidesWithSubEntities(e);
    //if (collidesWithSubentities)
    //{
    //  throw new InvalidDataException("Collides with subentities");
    //}
    //e.MovementModule.Friction = 0;

    //base.AddEntity(e);
    //MovementModule.Mass += e.MovementModule.Mass;
    //MovementModule.Thrust = e.MovementModule.Thrust;

    if (e is WorldEntity ee)
      ConnectToOthers(ee);
    else
      throw new NotImplementedException("Only WorldEntity supported in CompositeController");
    //e.Manager = this;
    base.Entities.Add(e);

  }

  public void SetEntities(List<IEntity> newEntities)
  {
    if (newEntities == null || newEntities.Count == 0)
    {
      return;
    }
    //base.SetEntities(newEntities);

    foreach (IEntity entity in newEntities)
    {
      //entity.MovementModule.Friction = 0;
      //entity.Rotation.Value = Rotation;
      //MovementModule.Mass += entity.Mass;
      //MovementModule.Thrust += entity.MovementModule.Thrust;
      AddEntity(entity);
    }
  }

  protected void ConnectToOthers(WorldEntity entity)
  {
    if (Entities.Count <= 0 || entity.IsFiller)
    {
      return;
    }

    var entityLinkModule = entity.GetModule<LinkModule>();
    if (entityLinkModule == null) return;

    foreach (WorldEntity e in Entities)
    {
      if (entity == e || e.IsFiller)
      {
        continue;
      }

      var eLinkModule = e.GetModule<LinkModule>();
      if (eLinkModule == null) continue;

      foreach (Link lE in eLinkModule.Links)
      {
        if (!lE.ConnectionAvailable)
        {
          continue;
        }
        foreach (Link lEntity in entityLinkModule.Links)
        {
          if (lEntity.ConnectionAvailable)
          {
            lE.ConnectTo(lEntity);
          }
        }
      }
    }
  }

  #region ConnectSeperatedEntities
  /*private List<HashSet<WorldEntity>> GetSetsOfEntities()
  {
      List<HashSet<WorldEntity>> sets = new List<HashSet<WorldEntity>>();
      //Entities.Sort((a, b) => a.Links.Count.CompareTo(a.Links.Count));
      foreach (WorldEntity e in entities)
      {
          bool containsEntity = false;
          foreach (HashSet<WorldEntity> s in sets)
              if (s.Contains(e))
                  containsEntity = true;
          if (!containsEntity)
          {
              HashSet<WorldEntity> set = new HashSet<WorldEntity>();
              set.Add(e);
              GetConnectedEntities(e, set);
              sets.Add(set);
          }

      }
      return sets;
  }
  private HashSet<WorldEntity> GetConnectedEntities(WorldEntity e, HashSet<WorldEntity> foundEntities)
  {
      foreach (Link l in e.Links)
          if (!l.ConnectionAvailable)
              if (!foundEntities.Contains(l.connection.Entity))
              {
                  foundEntities.Add(l.connection.Entity);
                  GetConnectedEntities(l.connection.Entity, foundEntities);
              }
      return foundEntities;
  }
  /*public bool Remove(IControllable c)
  {
      if (c is WorldEntity we)
      {
          if (we != null && Controllables.Remove(we))
          {
              foreach (Link l in we.Links)
                  if (!l.ConnectionAvailable && l.connection.Entity.Links.Count == 1)
                      ;// RemoveEntity(l.connection.Entity);
              if (we is Shooter s)
                  projectiles.Remove(s.Projectiles);

              foreach (Link l in we.Links) //remove filler links
                  if (!l.ConnectionAvailable)
                  {
                      if (l.connection.Entity.IsFiller)
                          Controllables.Remove(l.connection.Entity);
                      l.SeverConnection();
                  }
              List<HashSet<WorldEntity>> connectedEntities = GetSetsOfEntities();
              for (int i = 1; i < connectedEntities.Count; i++)
              {
                  WorldEntity[] tempEntities = new WorldEntity[connectedEntities[i].Count];
                  connectedEntities[i].CopyTo(tempEntities);
                  foreach (WorldEntity eSeperated in tempEntities)
                      Controllables.Remove(eSeperated);
                  EntityController ec = new EntityController(tempEntities, Rotation);

                  SeperatedEntities.Add(ec);
              }
              UpdatePosition();
              UpdateRadius();
              we.Manager = null;
              return true;
          }
          else
          {
              bool removed = false;
              foreach (EntityController ec in SeperatedEntities)
                  if (ec.Remove(we))
                      removed = true;
              return removed;
          }
      }
      else
          return false;
  }*/
  #endregion
  #region Links
  /*public void AddAvailableLinkDisplays()
  {
      List<WorldEntity> tempEntities = new List<WorldEntity>();
      foreach (WorldEntity e in Controllables)
      {
          if (!e.IsFiller)
          {
              List<WorldEntity> fillers = e.FillEmptyLinks();
              foreach (WorldEntity ee in fillers)
                  if (!Controllables.Contains(ee))
                      tempEntities.Add(ee);
          }
      }
      foreach (WorldEntity eT in tempEntities)
      {
          bool overlaps = false;
          foreach (WorldEntity eE in Controllables)
              if (eT.CollidesWith(eE))//eT.Contains(eE.Position) || eE.Contains(eT.Position))
                  overlaps = true;
          if (!overlaps)
              AddEntity(eT);
          else
              eT.Links[0].connection.SeverConnection();
      }
  }
  public void ClearAvailableLinks()
  {
      List<WorldEntity> tempEntities = new List<WorldEntity>();
      foreach (WorldEntity e in Controllables)
      {
          if (e.IsFiller)
          {
              foreach (Link l in e.Links)
                  l.SeverConnection();
              tempEntities.Add(e);
          }
      }
      foreach (WorldEntity e in tempEntities)
          Controllables.Remove(e);
      foreach (EntityController ec in SeperatedEntities)
          ec.ClearAvailableLinks();
      UpdateRadius();
  }
  public bool ReplaceEntity(WorldEntity eOld, WorldEntity eNew)
  {
      if (Controllables.Contains(eOld))
      {
          eNew.ConnectTo(eOld.Links[0].connection.Entity, eOld.Links[0].connection);
          Controllables.Remove(eOld);
          eOld.Links[0].SeverConnection();
          if (!AddEntity(eNew))
          {
              eOld.ConnectTo(eNew.Links[0].connection.Entity, eNew.Links[0].connection);
              AddEntity(eOld);
              return false;
          }

          return true;
      }
      return false;
  }
  */
  #endregion

  public void Collide(IEntity e)
  {
    /*
    //collission repulsion
    Vector2 vectorFromOther = e.Position - position;
    float distance = vectorFromOther.Length();
    vectorFromOther.Normalize();
    Vector2 collissionRepulsion = 0.5f * Vector2.Normalize(-vectorFromOther) * (Vector2.Dot(velocity, vectorFromOther) * Mass + Vector2.Dot(e.Velocity, -vectorFromOther) * e.Mass); //make velocity depend on position
    TotalExteriorForce += collissionRepulsion;

    //overlap repulsion
    float distance2 = (position - e.Position).Length();
    if (distance2 < 5)
        distance2 = 5;
    float radius = Radius * (e.Mass + Mass) / 2;
    Vector2 overlapRepulsion = 30f * Vector2.Normalize(position - e.Position) / distance2;
    TotalExteriorForce += overlapRepulsion;*/
  }
  public void Deprecate()
  {
    throw new NotImplementedException();
  }
}
