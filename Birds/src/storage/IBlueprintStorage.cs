using Birds.src.controllers.composite.blueprints;
using Birds.src.controllers.composite.blueprints.parts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Birds.src.storage
{
  public interface IBlueprintStorage
  {
    Task SaveBlueprintAsync(CompositeBlueprint blueprint);
    Task<CompositeBlueprint> LoadBlueprintAsync(string name);
    Task<List<string>> GetBlueprintNamesAsync();
    Task DeleteBlueprintAsync(string name);
    Task<bool> BlueprintExistsAsync(string name);
  }

  public interface IEntityPlacementStorage
  {
    Task SaveEntityPlacementsAsync(string blueprintName, List<EntityPlacement> placements);
    Task<List<EntityPlacement>> LoadEntityPlacementsAsync(string blueprintName);
    Task DeleteEntityPlacementsAsync(string blueprintName);
  }

  public interface IConnectionStorage
  {
    Task SaveConnectionsAsync(string blueprintName, List<Connection> connections);
    Task<List<Connection>> LoadConnectionsAsync(string blueprintName);
    Task DeleteConnectionsAsync(string blueprintName);
  }
}
