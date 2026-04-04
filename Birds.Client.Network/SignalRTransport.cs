using Birds.src.api.contracts;
using Birds.src.api.transport;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Birds.Client.Network;

public class SignalRTransport : INetworkTransport
{
  private HubConnection _hubConnection;
  private readonly string _serverUrl;

  public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

  public SignalRTransport(string serverUrl)
  {
    _serverUrl = serverUrl;
  }

  public async Task ConnectAsync()
  {
    _hubConnection = new HubConnectionBuilder()
        .WithUrl($"{_serverUrl}/gameHub")
        .WithAutomaticReconnect()
        .Build();

    _hubConnection.On<GameStateMessage>("ReceiveGameState", (message) =>
    {
      StateReceived?.Invoke(message);
    });

    await _hubConnection.StartAsync();
  }

  public async Task DisconnectAsync()
  {
    if (_hubConnection != null)
      await _hubConnection.StopAsync();
  }

  public async Task SendInputAsync(InputMessage input)
  {
    if (_hubConnection?.State == HubConnectionState.Connected)
      await _hubConnection.InvokeAsync("SendInput", input);
  }

  public event Action<GameStateMessage> StateReceived;
}
