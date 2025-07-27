using Birds.src.controllers.composite.blueprints.parts;
using System.Collections.Generic;

namespace Birds.src.controllers.composite.blueprints;

public class CompositeBlueprint
{
  public string Name{ get; set; }
  public List<EntityPlacement> Entities { get; set; }
  public List<Connection> Connections { get; set; }
}