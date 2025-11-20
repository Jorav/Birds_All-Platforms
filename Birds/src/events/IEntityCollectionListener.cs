using Birds.src.containers.entity;

namespace Birds.src.events;

public interface IEntityCollectionListener
{
  void OnEntityAdded(IEntity entity);
  void OnEntityRemoved(IEntity entity);
}
