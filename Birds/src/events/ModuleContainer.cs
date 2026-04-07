using Birds.src.containers.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Birds.src.events;

public abstract class ModuleContainer : IModuleContainer
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  private SyncedProperty<Vector2> _position;
  private SyncedProperty<float> _rotation;
  private SyncedProperty<float> _mass;
  private SyncedProperty<float> _radius;
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
  public SyncedProperty<float> Radius => _radius ??= new SyncedProperty<float>(1);
  public SyncedProperty<ID_OTHER> Team => _team ??= new SyncedProperty<ID_OTHER>();
  public SyncedProperty<Vector2> Velocity => _velocity ??= new SyncedProperty<Vector2>();
  public SyncedProperty<float> Scale => _scale ??= new SyncedProperty<float>(1);
  public SyncedProperty<float> Width => _width ??= new SyncedProperty<float>(1);
  public SyncedProperty<float> Height => _height ??= new SyncedProperty<float>(1);
  public SyncedProperty<float> Thrust => _thrust ??= new SyncedProperty<float>(1);
  public SyncedProperty<bool> ResolveInternalCollisions => _resolveInternalCollisions ??= new SyncedProperty<bool>(true);

  private ObservableCollection<IEntity> _entities = new();
  public ObservableCollection<IEntity> Entities => _entities;
  public List<IModuleContainer> Collisions { get; set; } = new(8);

  private Dictionary<Type, ModuleBase> modules = new Dictionary<Type, ModuleBase>();

  public ModuleContainer()
  {
    _entities.CollectionChanged += OnEntitiesCollectionChanged;
  }

  private void ResetProperties()
  {
    _position?.Reset();
    _velocity?.Reset();
    _rotation?.Reset(0);
    _mass?.Reset(1);
    _radius?.Reset(1);
    _team?.Reset();
    _scale?.Reset(1);
    _width?.Reset(1);
    _height?.Reset(1);
    _thrust?.Reset(1);
    _resolveInternalCollisions?.Reset(true);
  }

  private void OnEntitiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.NewItems != null)
    {
      foreach (IEntity entity in e.NewItems)
      {
        foreach (var module in modules.Values.OfType<IEntityCollectionListener>())
        {
          module.OnEntityAdded(entity);
          SyncWriteProperties();
        }
      }
    }

    if (e.OldItems != null)
    {
      foreach (IEntity entity in e.OldItems)
      {
        foreach (var module in modules.Values.OfType<IEntityCollectionListener>())
        {
          module.OnEntityRemoved(entity);
          SyncWriteProperties();
        }
      }
    }
  }

  public void SyncWriteProperties()
  {
    foreach (var module in modules.Values)
      module.SyncWriteProperties();
  }

  public void AddModule<T>(T module) where T : ModuleBase
  {
    module.Initialize(this);
    modules[typeof(T)] = module;
    module.SyncWriteProperties();
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

  public void ClearModules()
  {
    foreach (ModuleBase module in modules.Values.ToList())
    {
      module.Dispose();
    }
    modules.Clear();
  }

  public virtual void Update(GameTime gameTime)
  {
    foreach (var entity in _entities)
    {
      entity.Update(gameTime);
    }

    if (Game1.LOG_MODULE_PERFORMANCE)
    {
      foreach (var module in modules.Values)
      {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        module.UpdateModule(gameTime);
        sw.Stop();
        ModuleProfiler.Record(module.GetType(), sw.ElapsedTicks);
      }
    }
    else
    {
      foreach (var module in modules.Values)
      {
        module.UpdateModule(gameTime);
      }
    }
  }

  public virtual object Clone()
  {
    var cloned = (ModuleContainer)this.MemberwiseClone();
    cloned.modules = new Dictionary<Type, ModuleBase>();
    cloned._entities = new ObservableCollection<IEntity>();
    cloned._entities.CollectionChanged += cloned.OnEntitiesCollectionChanged;
    cloned.Collisions = new List<IModuleContainer>(8);

    if (_position != null) cloned._position = new SyncedProperty<Vector2>(_position.Value);
    if (_rotation != null) cloned._rotation = new SyncedProperty<float>(_rotation.Value);
    if (_mass != null) cloned._mass = new SyncedProperty<float>(_mass.Value);
    if (_radius != null) cloned._radius = new SyncedProperty<float>(_radius.Value);
    if (_team != null) cloned._team = new SyncedProperty<ID_OTHER>(_team.Value);
    if (_velocity != null) cloned._velocity = new SyncedProperty<Vector2>(Vector2.Zero);
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

    cloned.Entities.Set(_entities.Select(e => (IEntity)e.Clone()));

    cloned.SyncWriteProperties();
    return cloned;
  }

  public virtual void Dispose()
  {
    _entities.CollectionChanged -= OnEntitiesCollectionChanged;

    foreach (var module in modules.Values) module.Dispose();
    modules.Clear();

    foreach (IEntity entity in _entities) entity.Dispose();
    _entities.Clear();

    Collisions.Clear();

    ResetProperties();

    _entities.CollectionChanged += OnEntitiesCollectionChanged;
  }
}