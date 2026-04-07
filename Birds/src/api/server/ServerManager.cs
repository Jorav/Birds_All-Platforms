using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Birds.src.api.server;

public class ServerManager
{
  private readonly ServerRunner _runner = new();
  private Task _serverTask;
  private CancellationTokenSource _cts;

  public bool IsRunning { get; private set; }
  public string GetLocalServerAddress() => "localhost";
  public int GetLocalServerPort() => 9050;

  public async Task<bool> StartLocalServer()
  {
    if (IsRunning) return true;

    try
    {
      _cts = new CancellationTokenSource();
      _serverTask = Task.Run(() => _runner.Run(9050, _cts.Token));

      await Task.Delay(300);

      if (_serverTask.IsFaulted)
      {
        Debug.WriteLine($"Server faulted: {_serverTask.Exception?.InnerException?.Message}");
        return false;
      }

      IsRunning = true;
      return true;
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Failed to start server: {ex.Message}");
      return false;
    }
  }

  public void StopLocalServer()
  {
    if (!IsRunning) return;
    _cts?.Cancel();
    _cts?.Dispose();
    _cts = null;
    _serverTask = null;
    IsRunning = false;
  }
}
