using System;
using System.Collections.Generic;

namespace Birds.src.events;

public class SyncedProperty<T>
{
  private T _value;
  public event Action<T> ValueChanged;
  public bool IsDirty { get; private set; }

  public T Value
  {
    get => _value;
    set
    {
      if (!EqualityComparer<T>.Default.Equals(_value, value))
      {
        _value = value;
        IsDirty = true;
        ValueChanged?.Invoke(value);
      }
    }
  }
  public void Reset(T value = default)
  {
    _value = value;
    IsDirty = false;
    ValueChanged = null;
  }

  public SyncedProperty(T initialValue = default)
  {
    _value = initialValue;
  }

  public void ClearDirty() => IsDirty = false;

  public static implicit operator T(SyncedProperty<T> property) => property.Value;
}
