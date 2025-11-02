using Birds.src.containers.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Birds.src.events;

public abstract class ModuleContainer : IModuleContainer
{
  private SyncedProperty<Vector2> _position;
  private SyncedProperty<float> _rotation;
  private SyncedProperty<float> _mass;
  private SyncedProperty<float> _radius;
  private SyncedProperty<Color> _color;
  private SyncedProperty<ID_OTHER> _team;
  private SyncedProperty<Vector2> _velocity;
  private SyncedProperty<float> _scale;
  private SyncedProperty<float> _width;
  private SyncedProperty<float> _height;
  private SyncedProperty<float> _thrust;
  private SyncedProperty<bool> _resolveInternalCollisions;

  public SyncedProperty<Vector2> Position => _position ??= new SyncedProperty<Vector2>();
  public SyncedProperty<float> Rotation => _rotation ??= new SyncedProperty<float>(0);
  public SyncedProperty<float> Mass => _mass ??= new SyncedProperty<float>(1);
  public SyncedProperty<float> Radius => _radius ??= new SyncedProperty<float>(0);
  public SyncedProperty<Color> Color => _color ??= new SyncedProperty<Color>(Microsoft.Xna.Framework.Color.Red);
  public SyncedProperty<ID_OTHER> Team => _team ??= new SyncedProperty<ID_OTHER>();
  public SyncedProperty<Vector2> Velocity => _velocity ??= new SyncedProperty<Vector2>();
  public SyncedProperty<float> Scale => _scale ??= new SyncedProperty<float>(1);
  public SyncedProperty<float> Width => _width ??= new SyncedProperty<float>();
  public SyncedProperty<float> Height => _height ??= new SyncedProperty<float>();
  public SyncedProperty<float> Thrust => _thrust ??= new SyncedProperty<float>(1);
  public SyncedProperty<bool> ResolveInternalCollisions => _resolveInternalCollisions ??= new SyncedProperty<bool>(true);

  public List<IEntity> Entities { get; private set; } = new();
  public List<IModuleContainer> Collisions { get; } = new();

  private Dictionary<Type, ModuleBase> modules = new Dictionary<Type, ModuleBase>();

  public void AddModule<T>(T module) where T : ModuleBase
  {
    module.Initialize(this);
    modules[typeof(T)] = module;
  }

  public T GetModule<T>() where T : ModuleBase
  {
    if (modules.TryGetValue(typeof(T), out var module))
    {
      return module as T;
    }
    foreach (var kvp in modules)
    {
      if (kvp.Value is T matchingModule)
      {
        return matchingModule;
      }
    }
    return null;
  }

  public IEnumerable<TBase> GetAllModulesOfType<TBase>() where TBase : ModuleBase
  {
    foreach (var module in modules.Values)
    {
      if (module is TBase match)
      {
        yield return match;
      }
    }
  }

  public bool HasModule<T>() where T : ModuleBase
  {
    return modules.ContainsKey(typeof(T));
  }

  public virtual void Update(GameTime gameTime)
  {
    foreach (var entity in Entities)
    {
      entity.Update(gameTime);
    }
    foreach (var module in modules.Values)
    {
      module.UpdateModule(gameTime);
    }
  }

  public virtual object Clone()
  {
    var cloned = (ModuleContainer)this.MemberwiseClone();
    cloned.modules = new Dictionary<Type, ModuleBase>();
    cloned.Entities = new List<IEntity>();

    if (_position != null) cloned._position = new SyncedProperty<Vector2>(_position.Value);
    if (_rotation != null) cloned._rotation = new SyncedProperty<float>(_rotation.Value);
    if (_mass != null) cloned._mass = new SyncedProperty<float>(_mass.Value);
    if (_radius != null) cloned._radius = new SyncedProperty<float>(_radius.Value);
    if (_color != null) cloned._color = new SyncedProperty<Color>(_color.Value);
    if (_team != null) cloned._team = new SyncedProperty<ID_OTHER>(_team.Value);
    if (_velocity != null) cloned._velocity = new SyncedProperty<Vector2>(_velocity.Value);
    if (_scale != null) cloned._scale = new SyncedProperty<float>(_scale.Value);
    if (_width != null) cloned._width = new SyncedProperty<float>(_width.Value);
    if (_height != null) cloned._height = new SyncedProperty<float>(_height.Value);
    if (_thrust != null) cloned._thrust = new SyncedProperty<float>(_thrust.Value);
    if (_resolveInternalCollisions != null) cloned._resolveInternalCollisions = new SyncedProperty<bool>(_resolveInternalCollisions.Value);

    foreach (var kvp in modules)
    {
      var clonedModule = (ModuleBase)kvp.Value.Clone();
      clonedModule.Initialize(cloned);
      cloned.modules[clonedModule.GetType()] = clonedModule;
    }

    List<IEntity> clonedEntities = new List<IEntity>();
    foreach (IEntity entity in Entities)
    {
      clonedEntities.Add((IEntity)entity.Clone());
    }
    cloned.Entities = clonedEntities;

    return cloned;
  }

  public virtual void Dispose()
  {
    foreach (var module in modules.Values)
    {
      module.Dispose();
    }
    modules.Clear();
  }
}
