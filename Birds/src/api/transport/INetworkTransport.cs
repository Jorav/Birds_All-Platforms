using Birds.src.api.contracts;
using System.Threading.Tasks;

namespace Birds.src.api.transport;

public interface INetworkTransport
{
  Task ConnectAsync();
  Task DisconnectAsync();
  Task SendInputAsync(InputMessage input);
  void PollEvents();
}