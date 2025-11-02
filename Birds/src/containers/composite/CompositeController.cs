using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.modules.composite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Birds.src.containers.composite;

public class CompositeController : ModuleContainer, IEntity
{
  public Controller Manager { get; set; }

  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
  }

  public void AddEntity(IEntity e)
  {
    var linkModule = GetModule<LinkManagementModule>();
    if (linkModule != null)
      linkModule.AddEntity(e);
    else
      Entities.Add(e);
  }

  public void SetEntities(List<IEntity> newEntities)
  {
    if (newEntities == null || newEntities.Count == 0)
    {
      return;
    }

    foreach (IEntity entity in newEntities)
    {
      AddEntity(entity);
    }
  }

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

  public void Deprecate()
  {
    throw new NotImplementedException();
  }
}
