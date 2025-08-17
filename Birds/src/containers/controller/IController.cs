using Birds.src.containers.entity;
using System.Collections.Generic;

namespace Birds.src.containers.controller;

public interface IController
{
  public List<IEntity> Entities { get; }
  public void SetEntities(List<IEntity> newEntities);
  public void AddEntity(IEntity c);
}
