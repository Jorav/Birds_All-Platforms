using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Birds.src.events;

public abstract class ModuleBase
{
  public IModuleContainer container;
  private List<IPropertySync> _propertySyncs = new List<IPropertySync>(32);

  private static readonly Dictionary<PropertyInfo, Delegate> _getterCache = new Dictionary<PropertyInfo, Delegate>();
  private static readonly Dictionary<PropertyInfo, Delegate> _setterCache = new Dictionary<PropertyInfo, Delegate>();

  public virtual void Initialize(IModuleContainer container)
  {
    this.container = container;
    ConfigurePropertySync();
  }

  protected virtual void ConfigurePropertySync() { }

  protected void ReadSync<T>(
      Expression<Func<T>> moduleProperty,
      SyncedProperty<T> containerProperty)
  {
    var memberExpression = (MemberExpression)moduleProperty.Body;
    var propertyInfo = (PropertyInfo)memberExpression.Member;

    var setter = GetOrCreateSetter<T>(propertyInfo);

    containerProperty.ValueChanged += value => setter(this, value);
    setter(this, containerProperty.Value);
  }

  protected void WriteSync<T>(
      Expression<Func<T>> moduleProperty,
      SyncedProperty<T> containerProperty)
  {
    var memberExpression = (MemberExpression)moduleProperty.Body;
    var propertyInfo = (PropertyInfo)memberExpression.Member;
    var writeSync = new WriteSyncProperty<T>(this, propertyInfo, containerProperty);
    _propertySyncs.Add(writeSync);
  }

  protected void ReadWriteSync<T>(
      Expression<Func<T>> moduleProperty,
      SyncedProperty<T> containerProperty)
  {
    ReadSync(moduleProperty, containerProperty);
    WriteSync(moduleProperty, containerProperty);
  }

  private static Func<object, T> GetOrCreateGetter<T>(PropertyInfo propertyInfo)
  {
    if (!_getterCache.TryGetValue(propertyInfo, out var cached))
    {
      var instance = Expression.Parameter(typeof(object), "instance");
      var body = Expression.Property(
          Expression.Convert(instance, propertyInfo.DeclaringType),
          propertyInfo
      );
      cached = Expression.Lambda<Func<object, T>>(body, instance).Compile();
      _getterCache[propertyInfo] = cached;
    }
    return (Func<object, T>)cached;
  }

  private static Action<object, T> GetOrCreateSetter<T>(PropertyInfo propertyInfo)
  {
    if (!_setterCache.TryGetValue(propertyInfo, out var cached))
    {
      var instance = Expression.Parameter(typeof(object), "instance");
      var value = Expression.Parameter(typeof(T), "value");
      var body = Expression.Call(
          Expression.Convert(instance, propertyInfo.DeclaringType),
          propertyInfo.SetMethod,
          value
      );
      cached = Expression.Lambda<Action<object, T>>(body, instance, value).Compile();
      _setterCache[propertyInfo] = cached;
    }
    return (Action<object, T>)cached;
  }

  public virtual void UpdateModule(GameTime gameTime)
  {
    Update(gameTime);
    SyncWriteProperties();
  }

  protected abstract void Update(GameTime gameTime);

  public void SyncWriteProperties()
  {
    foreach (var propertySync in _propertySyncs)
    {
      propertySync.SyncToContainer();
    }
  }

  public virtual void Dispose()
  {
    _propertySyncs.Clear();
  }

  public virtual object Clone()
  {
    var cloned = (ModuleBase)this.MemberwiseClone();
    cloned.container = null;
    cloned._propertySyncs = new List<IPropertySync>(32);
    return cloned;
  }

  private interface IPropertySync
  {
    void SyncToContainer();
  }

  private class WriteSyncProperty<T> : IPropertySync
  {
    private readonly object _module;
    private readonly Func<object, T> _getter;
    private readonly SyncedProperty<T> _containerProperty;
    private T _lastValue;

    public WriteSyncProperty(object module, PropertyInfo moduleProperty, SyncedProperty<T> containerProperty)
    {
      _module = module;
      _getter = GetOrCreateGetter<T>(moduleProperty);
      _containerProperty = containerProperty;
      _lastValue = containerProperty.Value;
    }

    public void SyncToContainer()
    {
      var currentValue = _getter(_module);
      if (!EqualityComparer<T>.Default.Equals(currentValue, _lastValue))
      {
        _containerProperty.Value = currentValue;
        _lastValue = currentValue;
      }
    }
  }
}