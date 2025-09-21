using System.Collections.Generic;
using System;

public class ReactiveProperty<T>
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

  public ReactiveProperty(T initialValue = default)
  {
    _value = initialValue;
  }

  public static implicit operator T(ReactiveProperty<T> property)
  {
    return property.Value;
  }
}
