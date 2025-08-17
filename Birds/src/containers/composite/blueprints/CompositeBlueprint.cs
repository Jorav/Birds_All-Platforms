using Birds.src.containers.composite.blueprints.parts;
using System.Collections.Generic;

namespace Birds.src.containers.composite.blueprints;

public class CompositeBlueprint
{
  public string Name{ get; set; }
  public List<EntityPlacement> Entities { get; set; }
  public List<Connection> Connections { get; set; }
} 