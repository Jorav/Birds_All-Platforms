namespace Birds.src.utility;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ModuleProfiler
{
  private static Dictionary<string, long> _timings = new();
  private static int _frameCount = 0;

  public static void Record(Type moduleType, long ticks) => RecordInternal(moduleType.Name, ticks);
  public static void RecordSpecial(string name, long ticks) => RecordInternal(name, ticks);

  private static void RecordInternal(string key, long ticks)
  {
    if (!_timings.ContainsKey(key)) _timings[key] = 0;
    _timings[key] += ticks;
  }

  public static void Summary()
  {
    _frameCount++;
    if (_frameCount >= 60)
    {
      if (_timings.Count == 0) return;

      System.Diagnostics.Debug.WriteLine("\n--- TOP PERFORMANCE CULPRITS (Last 60 Frames) ---");
      var sorted = _timings.OrderByDescending(x => x.Value).Take(10); // Top 10

      foreach (var kvp in sorted)
      {
        double ms = (double)kvp.Value / System.TimeSpan.TicksPerMillisecond;
        System.Diagnostics.Debug.WriteLine($"{kvp.Key.PadRight(35)}: {ms:F2}ms");
      }
      _timings.Clear();
      _frameCount = 0;
    }
  }
}