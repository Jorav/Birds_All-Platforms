using Birds.src.bounding_areas;
using Birds.src.BVH;
using Birds.src.entities;
using Birds.src.modules.controller;
using Birds.src.modules.controller.steering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.controllers
{
  public interface IController
  {
    public List<IEntity> Entities { get; }
    public void SetEntities(List<IEntity> newEntities);
    public void AddEntity(IEntity c);
  }
}
