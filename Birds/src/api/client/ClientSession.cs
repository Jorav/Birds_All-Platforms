using System;

namespace Birds.src.api.client;

public class ClientSession
{
  public string ClientId { get; } = Guid.NewGuid().ToString();
  public string DisplayName { get; set; } = "Player";

  public static ClientSession Current { get; private set; }
  public static void Initialize() => Current = new ClientSession();
}
