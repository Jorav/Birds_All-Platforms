using Birds.src.containers.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Birds.src.events;

public interface IModuleContainer
{
  void AddModule<T>(T module) where T : ModuleBase;
  T GetModule<T>() where T : ModuleBase;
  IEnumerable<TBase> GetAllModulesOfType<TBase>() where TBase : ModuleBase;
  bool HasModule<T>() where T : ModuleBase;
  object Clone();

  ObservableCollection<IEntity> Entities { get; }
  List<IModuleContainer> Collisions { get; }

  SyncedProperty<Vector2> Position { get; }
  SyncedProperty<float> Rotation { get; }
  SyncedProperty<float> Mass { get; }
  SyncedProperty<float> Radius { get; }
  SyncedProperty<Microsoft.Xna.Framework.Color> Color { get; }
  SyncedProperty<ID_OTHER> Team { get; }
  SyncedProperty<Vector2> Velocity { get; }
  SyncedProperty<float> Scale { get; }
  SyncedProperty<float> Width { get; }
  SyncedProperty<float> Height { get; }
  SyncedProperty<float> Thrust { get; }
  SyncedProperty<bool> ResolveInternalCollisions { get; }
}
