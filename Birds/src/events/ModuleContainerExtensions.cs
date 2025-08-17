using Birds.src.modules.entity;
using Birds.src.modules.controller;
using Birds.src.modules;
using Microsoft.Xna.Framework;

namespace Birds.src.events;

public static class ModuleContainerExtensions
{
  public static void Accelerate(this IModuleContainer container, Vector2 directionalVector, float thrust)
  {
    var movement = container.GetModule<EntityMovementModule>() as IMovementModule ??
                  container.GetModule<ControllerMovementModule>() as IMovementModule;
    movement?.Accelerate(directionalVector, thrust);
  }

  public static void Accelerate(this IModuleContainer container, Vector2 directionalVector)
  {
    var movement = container.GetModule<EntityMovementModule>() as IMovementModule ??
                  container.GetModule<ControllerMovementModule>() as IMovementModule;
    movement?.Accelerate(directionalVector);
  }

  public static void AccelerateTo(this IModuleContainer container, Vector2 position, float thrust)
  {
    var movement = container.GetModule<EntityMovementModule>() as IMovementModule ??
                  container.GetModule<ControllerMovementModule>() as IMovementModule;
    movement?.AccelerateTo(position, thrust);
  }

  public static void RotateTo(this IModuleContainer container, Vector2 position)
  {
    var movement = container.GetModule<EntityMovementModule>() as IMovementModule ??
                  container.GetModule<ControllerMovementModule>() as IMovementModule;
    movement?.RotateTo(position);
  }

  public static Vector2 GetVelocity(this IModuleContainer container)
  {
    var entityMovement = container.GetModule<EntityMovementModule>();
    if (entityMovement != null) return entityMovement.Velocity;

    return Vector2.Zero;
  }
}

