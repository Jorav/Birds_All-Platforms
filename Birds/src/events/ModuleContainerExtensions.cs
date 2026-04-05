using Birds.src.modules;
using Birds.src.modules.controller;
using Birds.src.modules.entity;
using Birds.src.modules.shared.collision_detection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

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

  public static bool CollidesWith(this IModuleContainer container, IModuleContainer otherContainer)
  {
    var CDModule = container.GetModule<BaseCollisionDetectionModule>();
    var otherCDModule = otherContainer.GetModule<BaseCollisionDetectionModule>();
    return CDModule != null && otherCDModule != null && CDModule.CollidesWith(otherCDModule);
  }

  public static void Draw(this IModuleContainer container, SpriteBatch sb)
  {
    var renderModule = container.GetModule<DrawModule>() as IDrawModule ??
                      container.GetModule<GroupDrawModule>() as IDrawModule;
    renderModule?.Draw(sb);
  }

  public static IModuleContainer GetManager(this IModuleContainer container)
  {
    return container.GetModule<LinkModule>()?.Manager?.container;
  }

  public static List<ModuleContainer> FlattenHierarchy(this ModuleContainer container)
  {
    var result = new List<ModuleContainer> { container };

    foreach (var child in container.Entities.OfType<ModuleContainer>())
    {
      result.AddRange(child.FlattenHierarchy());
    }

    return result;
  }

  public static List<ModuleContainer> FlattenControllerHierarchy(this IEnumerable<ModuleContainer> controllers)
  {
    var result = new List<ModuleContainer>();

    foreach (var controller in controllers)
    {
      result.Add(controller);
      foreach (var child in controller.Entities.OfType<ModuleContainer>())
      {
        result.AddRange(child.FlattenHierarchy());
      }
    }

    return result;
  }

  public static string GetEntityId(this ModuleContainer entity)
  {
    return entity.GetHashCode().ToString();
  }
}

