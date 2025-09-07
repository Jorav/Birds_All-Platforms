using Birds.src.containers.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.events;
public interface IModuleContainer
{
  void AddModule<T>(T module) where T : ModuleBase;
  T GetModule<T>() where T : ModuleBase;
  bool HasModule<T>() where T : ModuleBase;

  List<IEntity> Entities { get; }

  // Reactive properties
  ReactiveProperty<Vector2> Position { get; }
  ReactiveProperty<float> Rotation { get; }
  ReactiveProperty<float> Mass { get; }
  ReactiveProperty<float> Radius { get; }
  ReactiveProperty<Microsoft.Xna.Framework.Color> Color { get; }
  ReactiveProperty<ID_OTHER> Team { get; }
  ReactiveProperty<Vector2> Velocity { get; }
  ReactiveProperty<float> Scale { get; }
  ReactiveProperty<float> Width { get; }
  ReactiveProperty<float> Height { get; }
  ReactiveProperty<float> Thrust { get; }
}