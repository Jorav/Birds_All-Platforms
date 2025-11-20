using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Birds.src.events;

public static class ObservableCollectionExtensions
{
  public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
  {
    if (items == null) return;

    foreach (var item in items)
    {
      collection.Add(item);
    }
  }

  public static void Set<T>(this ObservableCollection<T> collection, IEnumerable<T> newItems)
  {
    collection.Clear();

    foreach (var item in newItems)
    {
      collection.Add(item);
    }
  }
}
