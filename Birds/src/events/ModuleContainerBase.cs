using Birds.src.entities;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.events;
public abstract class ModuleContainerBase : IModuleContainer
{
  private ReactiveProperty<Vector2> _position;
  private ReactiveProperty<float> _rotation;
  private ReactiveProperty<float> _mass;
  private ReactiveProperty<float> _radius;
  private ReactiveProperty<Color> _color;
  private ReactiveProperty<ID_OTHER> _team;
  private ReactiveProperty<Vector2> _velocity;
  private ReactiveProperty<float> _scale;
  private ReactiveProperty<float> _width;
  private ReactiveProperty<float> _height;

  public ReactiveProperty<Vector2> Position => _position ??= new ReactiveProperty<Vector2>();
  public ReactiveProperty<float> Rotation => _rotation ??= new ReactiveProperty<float>();
  public ReactiveProperty<float> Mass => _mass ??= new ReactiveProperty<float>();
  public ReactiveProperty<float> Radius => _radius ??= new ReactiveProperty<float>();
  public ReactiveProperty<Color> Color => _color ??= new ReactiveProperty<Color>();
  public ReactiveProperty<ID_OTHER> Team => _team ??= new ReactiveProperty<ID_OTHER>();
  public ReactiveProperty<Vector2> Velocity => _velocity ??= new ReactiveProperty<Vector2>();
  public ReactiveProperty<float> Scale => _scale ??= new ReactiveProperty<float>();
  public ReactiveProperty<float> Width => _width ??= new ReactiveProperty<float>();
  public ReactiveProperty<float> Height => _height ??= new ReactiveProperty<float>();

  public List<IEntity> Entities { get; private set; } = new();

  private Dictionary<Type, ControllerModule> modules = new Dictionary<Type, ControllerModule>();

  public void AddModule<T>(T module) where T : ControllerModule
  {
    module.Initialize(this);
    modules[typeof(T)] = module;
  }

  public T GetModule<T>() where T : ControllerModule
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


  public bool HasModule<T>() where T : ControllerModule
  {
    return modules.ContainsKey(typeof(T));
  }

  public virtual void Update(GameTime gameTime)
  {
    foreach (var module in modules.Values)
    {
      module.UpdateModule(gameTime);
    }
  }

  public virtual object Clone()
  {
    var cloned = (ModuleContainerBase)this.MemberwiseClone();
    cloned.modules = new Dictionary<Type, ControllerModule>();
    cloned.Entities = new List<IEntity>();

    foreach (var kvp in modules)
    {
      var clonedModule = (ControllerModule)kvp.Value.Clone();
      clonedModule.Initialize(cloned);
      cloned.modules[clonedModule.GetType()] = clonedModule;
    }
    if (_position != null) cloned._position = new ReactiveProperty<Vector2>(_position.Value);
    if (_rotation != null) cloned._rotation = new ReactiveProperty<float>(_rotation.Value);
    if (_mass != null) cloned._mass = new ReactiveProperty<float>(_mass.Value);
    if (_radius != null) cloned._radius = new ReactiveProperty<float>(_radius.Value);
    if (_color != null) cloned._color = new ReactiveProperty<Color>(_color.Value);
    if (_team != null) cloned._team = new ReactiveProperty<ID_OTHER>(_team.Value);
    if (_velocity != null) cloned._velocity = new ReactiveProperty<Vector2>(_velocity.Value);
    if (_scale != null) cloned._scale = new ReactiveProperty<float>(_scale.Value);

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

