using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Birds.src.server;

public static class ServerManager
{
  private static Process _serverProcess;
  private const string SERVER_EXE = "Birds.Server.exe";
  private const string LOCAL_SERVER_ADDRESS = "localhost";
  private const int LOCAL_SERVER_PORT = 9050;

  public static async Task<bool> StartLocalServerAsync()
  {
    try
    {
      if (!File.Exists(SERVER_EXE))
      {
        throw new FileNotFoundException($"Server executable not found: {SERVER_EXE}");
      }

      _serverProcess = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = SERVER_EXE,
          UseShellExecute = false,
          CreateNoWindow = true,
          RedirectStandardOutput = true,
          RedirectStandardError = true
        }
      };

      _serverProcess.Start();
      await Task.Delay(2000);

      return _serverProcess != null && !_serverProcess.HasExited;
    }
    catch
    {
      return false;
    }
  }

  public static void StopLocalServer()
  {
    try
    {
      if (_serverProcess != null && !_serverProcess.HasExited)
      {
        _serverProcess.Kill();
        _serverProcess.Dispose();
        _serverProcess = null;
      }
    }
    catch { }
  }

  public static string GetLocalServerAddress() => LOCAL_SERVER_ADDRESS;
  public static int GetLocalServerPort() => LOCAL_SERVER_PORT;
}
