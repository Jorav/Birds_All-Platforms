using System.Collections.Generic;

namespace Birds.src.utility.factories.model;

public class WorldEntityConfigurationFile
{
  public WorldEntityConfiguration Default { get; set; }
  public Dictionary<string, WorldEntityConfiguration> Entities { get; set; } = new();
}