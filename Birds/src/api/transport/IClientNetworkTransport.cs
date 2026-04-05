using Birds.src.api.contracts;
using System;
using System.Threading.Tasks;

namespace Birds.src.api.transport;

public interface IClientNetworkTransport : INetworkTransport
{
  event Action<GameStateMessage> StateReceived;

  Task SendPlayerJoinAsync(PlayerJoinRequest joinRequest);
}