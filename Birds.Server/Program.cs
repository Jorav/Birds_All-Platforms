using Birds.server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace Birds.Server;

public class Program
{
  private static GameServer _gameServer;

  public static void Main(string[] args)
  {
    _gameServer = new GameServer();

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSignalR();
    builder.Services.AddCors(options =>
    {
      options.AddPolicy("AllowAll", policy =>
      {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
      });
    });

    builder.Services.AddSingleton(_gameServer);

    var app = builder.Build();

    app.UseCors("AllowAll");
    app.MapHub<GameHub>("/gameHub");

    // Start game loop in background
    _ = app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStarted.Register(() =>
    {
      _ = GameLoop();
    });

    app.Run("http://0.0.0.0:5000");
  }

  private static async Task GameLoop()
  {
    var sw = System.Diagnostics.Stopwatch.StartNew();
    const float tickRate = 1f / 20f;

    while (true)
    {
      var gameTime = new GameTime(sw.Elapsed, System.TimeSpan.FromSeconds(tickRate));
      _gameServer.Update(gameTime);
      await Task.Delay((int)(tickRate * 1000));
    }
  }

  public static GameServer GetGameServer() => _gameServer;
}
