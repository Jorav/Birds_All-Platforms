using Birds.src.modules.entity;
using Birds.src.modules.controller;
using Birds.src.modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.events;

public static class ModuleContainerExtensions
{
  public static void Accelerate(this IModuleContainer container, Vector2 directionalVector, float thrust)
  {
    var movement = container.GetModule<MovementModule>() as IMovementModule ??
                  container.GetModule<GroupMovementModule>() as IMovementModule;
    movement?.Accelerate(directionalVector, thrust);
  }

  public static void Accelerate(this IModuleContainer container, Vector2 directionalVector)
  {
    var movement = container.GetModule<MovementModule>() as IMovementModule ??
                  container.GetModule<GroupMovementModule>() as IMovementModule;
    movement?.Accelerate(directionalVector);
  }

  public static void AccelerateTo(this IModuleContainer container, Vector2 position, float thrust)
  {
    var movement = container.GetModule<MovementModule>() as IMovementModule ??
                  container.GetModule<GroupMovementModule>() as IMovementModule;
    movement?.AccelerateTo(position, thrust);
  }

  public static void RotateTo(this IModuleContainer container, Vector2 position)
  {
    var rotation = container.GetModule<RotationModule>() as IRotationModule ??
                  container.GetModule<GroupRotationModule>() as IRotationModule;
    rotation?.RotateTo(position);
  }

  public static Vector2 GetVelocity(this IModuleContainer container)
  {
    var entityMovement = container.GetModule<MovementModule>();
    if (entityMovement != null) return entityMovement.Velocity;

    return Vector2.Zero;
  }

  public static void Draw(this IModuleContainer container, SpriteBatch sb)
  {
    var renderModule = container.GetModule<DrawModule>() as IDrawModule ??
                      container.GetModule<GroupDrawModule>() as IDrawModule;
    renderModule?.Draw(sb);
  }
}

