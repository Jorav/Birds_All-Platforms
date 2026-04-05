using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.api.contracts;

public class PlayerJoinRequest
{
  public string PlayerId { get; set; }
  public string DisplayName { get; set; }
}
