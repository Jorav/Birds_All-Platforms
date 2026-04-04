using Birds.server;
using Birds.src.api.contracts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Birds.Server;

public class GameHub : Hub
{
  private static GameServer _gameServer;

  public async Task SendInput(InputMessage input)
  {
    _gameServer.ReceiveInput(input);
    await Task.CompletedTask;
  }

  public async Task SendGameState(GameStateMessage state)
  {
    await Clients.Caller.SendAsync("ReceiveGameState", state);
  }

  public override async Task OnConnectedAsync()
  {
    await base.OnConnectedAsync();
  }

  public override async Task OnDisconnectedAsync(Exception exception)
  {
    await base.OnDisconnectedAsync(exception);
  }
}
