using System.Collections.Generic;

namespace Birds.src.utility.factories.model.entity;

public class ModuleConfigurationEntry
{
  public ID_MODULE Type { get; set; }
  public Dictionary<string, object> Parameters { get; set; } = new();
}