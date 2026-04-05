using Birds.src.api.transport;
using Birds.src.network;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace Birds.Server;

public class Program
{
  private static GameServer _gameServer;
  private static IServerNetworkTransport _networkTransport;

  public static void Main(string[] args)
  {
    _networkTransport = new LiteNetLibServerTransport(9050);
    _gameServer = new GameServer(_networkTransport);

    _networkTransport.Start();
    Console.WriteLine("Game server started on port 9050");

    _ = Task.Run(GameLoop);

    Console.WriteLine("Press any key to stop server...");
    Console.ReadKey();

    _networkTransport.Stop();
  }

  private static async Task GameLoop()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();
    const float tickRate = 1f / 20f;

    while (true)
    {
      _networkTransport.PollEvents();

      var gameTime = new GameTime(sw.Elapsed, System.TimeSpan.FromSeconds(tickRate));
      _gameServer.Update(gameTime);

      await Task.Delay((int)(tickRate * 1000));
    }
  }
}
