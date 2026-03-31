using Microsoft.Xna.Framework;

namespace Birds.src.utility;

public class LongPressTimer
{
  public double Threshold { get; set; }
  public double CurrentHoldTime { get; private set; }
  public bool IsBeingHeld { get; private set; }
  public bool JustTriggered { get; private set; }
  public bool HasTriggered { get; private set; }

  public LongPressTimer(double threshold = 600)
  {
    Threshold = threshold;
  }

  public void Start()
  {
    IsBeingHeld = true;
    CurrentHoldTime = 0;
    HasTriggered = false;
    JustTriggered = false;
  }

  public void Stop()
  {
    IsBeingHeld = false;
    CurrentHoldTime = 0;
    HasTriggered = false;
    JustTriggered = false;
  }

  public void Update(GameTime gameTime)
  {
    JustTriggered = false;

    if (!IsBeingHeld || HasTriggered) return;

    CurrentHoldTime += gameTime.ElapsedGameTime.TotalMilliseconds;

    if (CurrentHoldTime >= Threshold)
    {
      HasTriggered = true;
      JustTriggered = true;
    }
  }
}