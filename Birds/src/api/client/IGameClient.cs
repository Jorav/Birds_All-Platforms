using Birds.src.api.contracts;
using System;
using System.Threading.Tasks;

namespace Birds.src.api.client;

public interface IGameClient
{
  Task ConnectAsync();
  Task DisconnectAsync();
  Task SendInputAsync(InputMessage input);

  event Action<GameStateMessage> StateReceived;
  event Action<string> Disconnected;
}
