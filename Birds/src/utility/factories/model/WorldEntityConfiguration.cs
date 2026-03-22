using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.utility.factories.model;

public class WorldEntityConfiguration
{
  public Dictionary<string, object> Properties { get; set; } = new();
  public List<ModuleConfigurationEntry> Modules { get; set; } = new();
}