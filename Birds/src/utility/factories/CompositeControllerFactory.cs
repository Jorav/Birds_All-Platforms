using Microsoft.Xna.Framework;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using Birds.src.entities;
using Birds.src.controllers.composite;
using Birds.src.controllers.composite.blueprints;
using Birds.src.controllers.composite.blueprints.parts;
using Birds.src.factories;
using Birds.src.storage;
using Birds.src.storage.implementations;
using System.Linq;

namespace Birds.src.factories
{
  public static class CompositeControllerFactory
  {
    public static Stack<CompositeController> availableEntities = new();
    private static IBlueprintStorage _storage = new JsonBlueprintStorage();

    public static CompositeController GetComposite(Vector2 position, string blueprintName)
    {
      CompositeController compositeController;
      if (availableEntities.Count > 0)
      {
        compositeController = availableEntities.Pop();
      }
      else
      {
        compositeController = new CompositeController();
      }
      var blueprint = GetBlueprintByName(blueprintName);
      var entities = BlueprintFactory.CreateFromBlueprint(blueprint, position);

      var iEntities = entities.Cast<IEntity>().ToList();
      compositeController.MovementModule = MovementModuleFactory.GetMovementModule();
      compositeController.SetEntities(iEntities);
      compositeController.Position = position;
      return compositeController;
    }

    private static CompositeController GetComposite(Vector2 position, ID_COMPOSITE id)
    {
      string blueprintName = GetBlueprintNameFromId(id);
      return GetComposite(position, blueprintName);
    }

    public static List<CompositeController> CreateComposites(Vector2 position, int numberOfEntities, ID_COMPOSITE id)
    {
      List<CompositeController> composites = new List<CompositeController>();
      for (int i = 0; i < numberOfEntities; i++)
      {
        composites.Add(GetComposite(position, id));
      }
      return composites;
    }
    private static CompositeBlueprint GetBlueprintByName(string blueprintName)
    {
      var premadeBlueprint = TryGetPremadeBlueprint(blueprintName);
      if (premadeBlueprint != null)
        return premadeBlueprint;

      try
      {
        return _storage.LoadBlueprintAsync(blueprintName).Result;
      }
      catch (Exception ex)
      {
        throw new ArgumentException($"Blueprint '{blueprintName}' not found in premade blueprints or storage. {ex.Message}");
      }
    }

    private static string GetBlueprintNameFromId(ID_COMPOSITE id)
    {
      switch (id)
      {
        case ID_COMPOSITE.DEFAULT_SINGLE:
          return "Single Entity";
        case ID_COMPOSITE.DEFAULT_COMBINED:
          return "Cross Shape";
        default:
          throw new NotImplementedException($"Blueprint name not mapped for {id}");
      }
    }

    private static CompositeBlueprint TryGetPremadeBlueprint(string blueprintName)
    {
      switch (blueprintName)
      {
        case "Single Entity":
          return CreateSingleEntityBlueprint();
        case "Cross Shape":
          return CreateCrossShapeBlueprint();
        default:
          return null;
      }
    }

    private static CompositeBlueprint CreateSingleEntityBlueprint()
    {
      return new CompositeBlueprint
      {
        Name = "Single Entity",
        Entities = new List<EntityPlacement>
                {
                    new EntityPlacement { Id = 0, EntityType = ID_ENTITY.DEFAULT }
                },
        Connections = new List<Connection>()
      };
    }

    private static CompositeBlueprint CreateCrossShapeBlueprint()
    {
      return new CompositeBlueprint
      {
        Name = "Cross Shape",
        Entities = new List<EntityPlacement>
                {
                    new EntityPlacement { Id = 0, EntityType = ID_ENTITY.DEFAULT }, // Center
                    new EntityPlacement { Id = 1, EntityType = ID_ENTITY.DEFAULT }, // Top
                    new EntityPlacement { Id = 2, EntityType = ID_ENTITY.DEFAULT }, // Right
                    new EntityPlacement { Id = 3, EntityType = ID_ENTITY.DEFAULT }, // Bottom
                    new EntityPlacement { Id = 4, EntityType = ID_ENTITY.DEFAULT }  // Left
                },
        Connections = new List<Connection>
                {
                    new Connection { EntityId1 = 0, EntityId2 = 1, LinkIndex1 = 0, LinkIndex2 = 2 }, // Center top → Top bottom
                    new Connection { EntityId1 = 0, EntityId2 = 2, LinkIndex1 = 1, LinkIndex2 = 3 }, // Center right → Right left
                    new Connection { EntityId1 = 0, EntityId2 = 3, LinkIndex1 = 2, LinkIndex2 = 0 }, // Center bottom → Bottom top
                    new Connection { EntityId1 = 0, EntityId2 = 4, LinkIndex1 = 3, LinkIndex2 = 1 }  // Center left → Left right
                }
      };
    }
  }
}
