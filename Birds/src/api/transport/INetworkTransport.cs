using Birds.src.api.contracts;
using System;
using System.Threading.Tasks;

namespace Birds.src.api.transport;

public interface INetworkTransport
{
  bool IsConnected { get; }
  Task ConnectAsync();
  Task DisconnectAsync();
  Task SendInputAsync(InputMessage input);
  event Action<GameStateMessage> StateReceived;
}
