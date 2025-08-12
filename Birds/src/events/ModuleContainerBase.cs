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

  public ReactiveProperty<Vector2> Position => _position ??= new ReactiveProperty<Vector2>();
  public ReactiveProperty<float> Rotation => _rotation ??= new ReactiveProperty<float>();
  public ReactiveProperty<float> Mass => _mass ??= new ReactiveProperty<float>(1f);
  public ReactiveProperty<float> Radius => _radius ??= new ReactiveProperty<float>(10f);
  public ReactiveProperty<Color> Color => _color ??= new ReactiveProperty<Color>(Microsoft.Xna.Framework.Color.White);
  public ReactiveProperty<ID_OTHER> Team => _team ??= new ReactiveProperty<ID_OTHER>();
  public ReactiveProperty<Vector2> Velocity => _velocity ??= new ReactiveProperty<Vector2>();
  public ReactiveProperty<float> Scale => _scale ??= new ReactiveProperty<float>(1f);

  public List<IEntity> Entities { get; } = new();

  private Dictionary<Type, ControllerModule> modules = new Dictionary<Type, ControllerModule>();

  public T AddModule<T>(T module) where T : ControllerModule
  {
    module.Initialize(this);
    modules[typeof(T)] = module;
    return module;
  }

  public T GetModule<T>() where T : ControllerModule
  {
    modules.TryGetValue(typeof(T), out var module);
    return module as T;
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

  public virtual void Dispose()
  {
    foreach (var module in modules.Values)
    {
      module.Dispose();
    }
    modules.Clear();
  }
}

