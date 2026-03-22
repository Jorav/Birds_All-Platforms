using Birds.src.modules.entity;
using System.Collections.Generic;

namespace Birds.src.containers.composite;

public class LinkModuleParameters
{
  public List<LinkConfiguration> Links { get; set; } = new();
}