using Birds.src.api.contracts;
using System;

namespace Birds.src.api.transport;

public interface IServerNetworkTransport : INetworkTransport
{
  event Action<string> PlayerConnected;
  event Action<string> PlayerDisconnected;
  event Action<InputMessage> InputReceived;
  event Action<PlayerJoinRequest> PlayerJoinRequested;

  void SendGameState(GameStateMessage state, string playerId);
  void Start();
  void Stop();
}