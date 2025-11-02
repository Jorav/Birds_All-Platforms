using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Birds.src.events;

public abstract class ModuleBase
{
  protected IModuleContainer container;
  private List<IPropertySync> _propertySyncs = new List<IPropertySync>();

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

    containerProperty.ValueChanged += value => propertyInfo.SetValue(this, value);
    propertyInfo.SetValue(this, containerProperty.Value);
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

  public virtual void UpdateModule(GameTime gameTime)
  {
    Update(gameTime);
    SyncWriteProperties();
  }

  protected abstract void Update(GameTime gameTime);

  private void SyncWriteProperties()
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
    cloned._propertySyncs = new List<IPropertySync>();
    return cloned;
  }

  private interface IPropertySync
  {
    void SyncToContainer();
  }

  private class WriteSyncProperty<T> : IPropertySync
  {
    private readonly object _module;
    private readonly PropertyInfo _moduleProperty;
    private readonly SyncedProperty<T> _containerProperty;
    private T _lastValue;

    public WriteSyncProperty(object module, PropertyInfo moduleProperty, SyncedProperty<T> containerProperty)
    {
      _module = module;
      _moduleProperty = moduleProperty;
      _containerProperty = containerProperty;
      _lastValue = containerProperty.Value;
    }

    public void SyncToContainer()
    {
      var currentValue = (T)_moduleProperty.GetValue(_module);

      if (!EqualityComparer<T>.Default.Equals(currentValue, _lastValue))
      {
        _containerProperty.Value = currentValue;
        _lastValue = currentValue;
      }
    }
  }
}
