using Birds.src.entities;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Birds.src.modules.events;
public interface IModuleContainer
{
  T AddModule<T>(T module) where T : ControllerModule;
  T GetModule<T>() where T : ControllerModule;
  bool HasModule<T>() where T : ControllerModule;

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
}