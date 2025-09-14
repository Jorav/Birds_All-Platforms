using Birds.src.factories;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System;

namespace Birds.src.modules.entity;
public class MovementModule : ModuleBase, IMovementModule
{
  public virtual float Mass { get; set; }
  public virtual float Thrust { get; set; }
  public virtual Vector2 Position { get; set; } = new();
  protected Vector2 position;

  protected Vector2 velocity = new();
  public virtual Vector2 Velocity { get { return velocity; } set { velocity = value; } }
  public virtual float Friction { get; set; } = 0.1f;// percent, where 0.1f = 10% friction
  public Vector2 TotalExteriorForce;

  protected override void ConfigurePropertySync()
  {
    ReadWriteSync(() => Position, container.Position);
    ReadWriteSync(() => Mass, container.Mass);
    ReadWriteSync(() => Thrust, container.Thrust);
  }
  public override void Initialize(IModuleContainer container)
  {
    base.Initialize(container);
    Mass = 1;
    Thrust = 1;
  }

  public void AccelerateTo(Vector2 position, float thrust)
  {
    Accelerate(position - Position, thrust);
  }

  public void Accelerate(Vector2 directionalVector, float thrust)
  {
    Vector2 direction = new Vector2(directionalVector.X, directionalVector.Y);
    direction.Normalize();
    TotalExteriorForce += direction * thrust;
  }

  public void Accelerate(Vector2 directionalVector)
  {
    Accelerate(directionalVector, Thrust);
  }

  public Vector2 VelocityAlongVector(Vector2 directionalVector)
  {
    directionalVector = new Vector2(directionalVector.X, directionalVector.Y);//unnecessary?
    directionalVector.Normalize();
    return Vector2.Dot(Velocity, directionalVector) / Vector2.Dot(directionalVector, directionalVector) * directionalVector;
  }

  protected override void Update(GameTime gameTime)
  {
    Vector2 FrictionForce = (Velocity * Mass + TotalExteriorForce) * Friction * (float)Game1.timeStep * 60;
    Velocity = Velocity + (TotalExteriorForce - FrictionForce) / Mass * (float)Game1.timeStep * 60;
    Position += Velocity * (float)Game1.timeStep * 60;
    TotalExteriorForce = Vector2.Zero;
  }

  public Vector2 CalculateCollissionRepulsion(MovementModule m)
  {
    Vector2 vectorFromOther = m.Position - Position;
    float distance = vectorFromOther.Length();
    vectorFromOther.Normalize();
    Vector2 collissionRepulsion = 0.5f * Vector2.Normalize(-vectorFromOther) * (Vector2.Dot(Velocity, vectorFromOther) * Mass + Vector2.Dot(m.Velocity, -vectorFromOther) * m.Mass); //make velocity depend on position
    return collissionRepulsion;
  }

  public override object Clone()
  {
    var cloned = new MovementModule();
    cloned.Mass = this.Mass;
    cloned.Thrust = this.Thrust;
    cloned.Friction = this.Friction;
    cloned.Velocity = Vector2.Zero;
    cloned.Position = this.Position;
    cloned.TotalExteriorForce = Vector2.Zero;
    return cloned;
  }
}
