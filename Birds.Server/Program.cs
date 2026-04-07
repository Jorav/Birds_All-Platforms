using Birds.src.api.server;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Birds.Server;

public class Program
{
  public static async Task Main(string[] args)
  {
    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
      e.Cancel = true;
      cts.Cancel();
    };

    await new ServerRunner().Run(port: 9050, cts.Token);
  }
}
