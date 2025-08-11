using Microsoft.Xna.Framework;

namespace Birds.src.modules
{
  public interface IMovementModule
  {
    void Accelerate(Vector2 directionalVector, float thrust);
    void Accelerate(Vector2 directionalVector);
    void RotateTo(Vector2 position);
  }
}
