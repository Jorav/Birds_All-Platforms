using Birds.src.api.transport;
using Birds.src.network;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Birds.src.api.server;

public class ServerRunner
{
  private GameServer _gameServer;
  private IServerNetworkTransport _networkTransport;

  public async Task Run(int port, CancellationToken token)
  {
    _networkTransport = new LiteNetLibServerTransport(port);
    _gameServer = new GameServer(_networkTransport);

    _networkTransport.Start();
    Debug.WriteLine($"Game server started on port {port}");

    var sw = Stopwatch.StartNew();
    const float tickRate = 1f / 20f;
    var tickDelay = TimeSpan.FromSeconds(tickRate);

    while (!token.IsCancellationRequested)
    {
      _networkTransport.PollEvents();
      _gameServer.Update(new GameTime(sw.Elapsed, tickDelay));
      await Task.Delay(tickDelay, token).ConfigureAwait(false);
    }

    _networkTransport.Stop();
    Debug.WriteLine("Game server stopped.");
  }
}
