using Birds.src.api.contracts;
using System;
using System.Threading.Tasks;

namespace Birds.src.api.transport;

public interface IClientNetworkTransport : INetworkTransport
{
  event Action<GameStateMessage> StateReceived;
  event Action<ControllerSpawnMessage> ControllerSpawnReceived;

  Task ConnectAsync();
  Task DisconnectAsync();
  Task SendPlayerJoinAsync(PlayerJoinRequest joinRequest);
  Task SendInputAsync(InputMessage input);
}