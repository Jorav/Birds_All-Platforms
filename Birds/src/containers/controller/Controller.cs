using System.Collections.Generic;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.utility;

namespace Birds.src.containers.controller;

public class Controller : ModuleContainer
{
  public ID_CONTROLLER ControllerId { get; set; }
  public Controller(List<IEntity> entities) : base()
  {
    Entities.AddRange(entities);
  }

  public override void Dispose()
  {
    base.Dispose();
    //Add on stack
  }
}
