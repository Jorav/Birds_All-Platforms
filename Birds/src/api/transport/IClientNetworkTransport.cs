using Birds.src.api.contracts;
using System;
using System.Threading.Tasks;

namespace Birds.src.api.transport;

public interface IClientNetworkTransport : INetworkTransport
{
  event Action<GameStateMessage> StateReceived;
  event Action<ControllerSpawnMessage> ControllerSpawnReceived;

  Task Connect();
  Task Disconnect();
  void SendPlayerJoin(PlayerJoinRequest joinRequest);
  void SendInput(InputMessage input);
}