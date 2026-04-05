using Birds.src.api.contracts;
using System.Threading.Tasks;

namespace Birds.src.api.transport;

public interface INetworkTransport
{
  void PollEvents();
}