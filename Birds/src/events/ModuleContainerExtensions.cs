using Birds.src.modules.entity;
using Birds.src.modules.controller;
using Birds.src.modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.modules.shared.collision_detection;

namespace Birds.src.events;

public static class ModuleContainerExtensions
{
  public static void Accelerate(this IModuleContainer container, Vector2 directionalVector, float thrust)
  {
    var movement = container.GetModule<MovementModule>();
    movement?.Accelerate(directionalVector, thrust);
  }

  public static void Accelerate(this IModuleContainer container, Vector2 directionalVector)
  {
    var movement = container.GetModule<MovementModule>();
    movement?.Accelerate(directionalVector);
  }

  public static void AccelerateTo(this IModuleContainer container, Vector2 position, float thrust)
  {
    var movement = container.GetModule<MovementModule>();
    movement?.AccelerateTo(position, thrust);
  }

  public static void RotateTo(this IModuleContainer container, Vector2 position)
  {
    var rotation = container.GetModule<RotationModuleBase>();

    rotation?.RotateTo(position);
  }

  public static bool Contains(this IModuleContainer container, Vector2 position)
  {
    var CDModule = container.GetModule<BaseCollisionDetectionModule>();
    return CDModule.Contains(position);
  }

  public static void Draw(this IModuleContainer container, SpriteBatch sb)
  {
    var renderModule = container.GetModule<DrawModule>() as IDrawModule ??
                      container.GetModule<GroupDrawModule>() as IDrawModule;
    renderModule?.Draw(sb);
  }
}

