using System;
using System.Diagnostics;

namespace Birds.Desktop;

static class Program
{
  [STAThread]
  static void Main()
  {
    try
    {
      using var game = new Birds.src.Game1();
      game.Run();
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"Fatal error: {ex}");
      Debug.WriteLine(ex.StackTrace);
    }
  }
}
