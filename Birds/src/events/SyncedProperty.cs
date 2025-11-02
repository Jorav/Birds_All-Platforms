using System;
using System.Collections.Generic;

namespace Birds.src.events;

public class SyncedProperty<T>
{
  private T _value;
  public event Action<T> ValueChanged;

  public T Value
  {
    get => _value;
    set
    {
      if (!EqualityComparer<T>.Default.Equals(_value, value))
      {
        _value = value;
        ValueChanged?.Invoke(value);
      }
    }
  }

  public SyncedProperty(T initialValue = default)
  {
    _value = initialValue;
  }

  public static implicit operator T(SyncedProperty<T> property) => property.Value;
}
