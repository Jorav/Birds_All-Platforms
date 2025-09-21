using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Birds.src.modules.events;
public abstract class ControllerModule
{
  protected IModuleContainer container;
  private List<IWriteSyncProperty> _writeSyncProperties = new List<IWriteSyncProperty>();

  public virtual void Initialize(IModuleContainer container)
  {
    this.container = container;
    ConfigurePropertySync();
  }

  protected virtual void ConfigurePropertySync() { }

  protected void ReadSync<T>(
      Expression<Func<T>> moduleProperty,
      ReactiveProperty<T> containerProperty)
  {
    var memberExpression = (MemberExpression)moduleProperty.Body;
    var propertyInfo = (PropertyInfo)memberExpression.Member;

    containerProperty.ValueChanged += value => propertyInfo.SetValue(this, value);

    propertyInfo.SetValue(this, containerProperty.Value);
  }

  protected void WriteSync<T>(
      Expression<Func<T>> moduleProperty,
      ReactiveProperty<T> containerProperty)
  {
    var memberExpression = (MemberExpression)moduleProperty.Body;
    var propertyInfo = (PropertyInfo)memberExpression.Member;

    var writeSyncProp = new WriteSyncProperty<T>(this, propertyInfo, containerProperty);
    _writeSyncProperties.Add(writeSyncProp);
  }

  protected void ReadWriteSync<T>(
      Expression<Func<T>> moduleProperty,
      ReactiveProperty<T> containerProperty)
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
    foreach (var writeSyncProp in _writeSyncProperties)
    {
      writeSyncProp.SyncToContainer();
    }
  }

  public virtual void Dispose()
  {
    _writeSyncProperties.Clear();
  }

  private interface IWriteSyncProperty
  {
    void SyncToContainer();
  }

  private class WriteSyncProperty<T> : IWriteSyncProperty
  {
    private readonly object _module;
    private readonly PropertyInfo _moduleProperty;
    private readonly ReactiveProperty<T> _containerProperty;
    private T _lastValue;

    public WriteSyncProperty(object module, PropertyInfo moduleProperty, ReactiveProperty<T> containerProperty)
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

