using System.Diagnostics;
using Birds.src.utility;

namespace Birds.src.utility;

public class DoubleClickHelper
{
  private readonly Stopwatch timer = new Stopwatch();
  private readonly int thresholdMs;
  private bool isWaitingForSecondClick = false;

  public DoubleClickHelper(int thresholdMs = 300)
  {
    this.thresholdMs = thresholdMs;
  }

  public bool CheckDoubleClick(bool isPressed, bool targetClicked)
  {
    if (timer.IsRunning && timer.ElapsedMilliseconds >= thresholdMs)
    {
      Reset();
    }

    if (!Input.WasJustPressed)
    {
      return false;
    }

    if (!targetClicked)
    {
      Reset();
      return false;
    }

    if (isWaitingForSecondClick && timer.ElapsedMilliseconds < thresholdMs)
    {
      Reset();
      return true;
    }
    else
    {
      timer.Restart();
      isWaitingForSecondClick = true;
      return false;
    }
  }

  public void Reset()
  {
    timer.Stop();
    timer.Reset();
    isWaitingForSecondClick = false;
  }

  public bool IsWaitingForSecondClick => isWaitingForSecondClick && timer.IsRunning;
}