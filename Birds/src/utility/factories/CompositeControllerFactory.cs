using Microsoft.Xna.Framework;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Birds.src.containers.composite.blueprints;
using Birds.src.containers.composite.blueprints.parts;
using Birds.src.containers.entity;
using Birds.src.containers.composite;
using Birds.src.modules.controller;
using Birds.src.modules.composite;
using Birds.src.modules.entity.collision_handling;
using Birds.src.modules.shared.collision_detection;
using Birds.src.collision.BVH;
using Birds.src.visual;
using Birds.src.utility.factories;
using Birds.src.events;

namespace Birds.src.factories;

public static class CompositeControllerFactory
{
  public static Stack<CompositeController> availableEntities = new(100);
  public static Dictionary<string, ISprite> Previews { get; set; } = new();

  public const string DEFAULT_SINGLE = "HULL_RECTANGULAR_BAD";
  public const string DEFAULT_CROSS = "Cross Shape";

  public static CompositeController GetComposite(Vector2 position, string blueprintName, bool useGeometricCenter = false)
  {
    CompositeController compositeController = availableEntities.Count > 0 ? availableEntities.Pop() : new CompositeController();

    var blueprint = GetBlueprintByName(blueprintName);
    var entities = BlueprintFactory.CreateFromBlueprint(blueprint, position, useGeometricCenter);

    compositeController.Position.Value = position;
    compositeController.Entities.Set(entities.Cast<IEntity>().ToList());
    SetCompositeModules(compositeController, ID_COMPOSITE.DEFAULT);

    return compositeController;
  }

  public static List<IEntity> CreateComposites(Vector2 position, int numberOfComposites, string id)
  {
    List<IEntity> returnedList = new List<IEntity>();
    CompositeController composite = GetComposite(position, id);
    returnedList.Add(composite);

    if (numberOfComposites > 1)
    {
      Random rnd = new Random();
      for (int i = 0; i < numberOfComposites - 1; i++)
      {
        float rRadius = (float)(rnd.NextDouble() * composite.Radius * 2 * Math.Sqrt(numberOfComposites));
        float rAngle = (float)(rnd.NextDouble() * 2 * Math.PI);
        Vector2 compositePosition = new Vector2((float)Math.Cos(rAngle), (float)Math.Sin(rAngle)) * rRadius + position;
        returnedList.Add(GetComposite(compositePosition, id));
      }
    }
    return returnedList;
  }

  private static void SetCompositeModules(CompositeController composite, ID_COMPOSITE id)
  {
    composite.ClearModules();
    composite.AddModule(new SubEntityVelocityReseter());
    composite.AddModule(new CohesiveGroupRotationModule());
    composite.AddModule(new CompositeMovementModule());
    composite.AddModule(new LinkManagementModule());
    composite.AddModule(new GroupMassModule());
    composite.AddModule(new GroupWeightedPositionModule());
    composite.AddModule(new GroupRadiusModule());
    composite.AddModule(new GroupThrustModule());
    composite.AddModule(new GroupCollisionDetectionModule(new ListCollisionStructure(), false));
    composite.AddModule(new SubEntityCollisionExtractionModule());
    composite.AddModule(GetCollisionHandler());
    composite.AddModule(new GroupDrawModule());
  }

  public static CollisionHandlerModule GetCollisionHandler()
  {
    var collisionHandler = new CollisionHandlerModule();
    collisionHandler.AddResponse(new MomentumTransfer());
    collisionHandler.AddResponse(new OverlapRepulsion());
    return collisionHandler;
  }

  private static CompositeBlueprint GetBlueprintByName(string blueprintName)
  {
    var premade = TryGetPremadeBlueprint(blueprintName);
    if (premade != null) return premade;

    try
    {
      return BlueprintFactory.LoadBlueprintAsync(blueprintName).Result;
    }
    catch (Exception ex)
    {
      throw new ArgumentException($"Blueprint '{blueprintName}' not found. {ex.Message}");
    }
  }

  private static CompositeBlueprint TryGetPremadeBlueprint(string blueprintName)
  {
    if (blueprintName == DEFAULT_CROSS) return CreateCrossShapeBlueprint();

    if (Enum.TryParse(blueprintName, out ID_ENTITY id))
    {
      if (WorldEntityLoader.Hulls.Contains(id) || id == ID_ENTITY.HULL_RECTANGULAR_BAD)
      {
        return CreateSingleEntityBlueprint(id);
      }
    }
    return null;
  }

  public static async Task InitializePreviews()
  {
    var savedNames = await BlueprintFactory.GetBlueprintNamesAsync();
    var blueprintNames = WorldEntityLoader.Hulls.Select(id => id.ToString()).ToList();

    foreach (var name in savedNames)
    {
      if (!blueprintNames.Contains(name)) blueprintNames.Add(name);
    }

    foreach (var name in blueprintNames)
    {
      var tempComposite = GetComposite(Vector2.Zero, name, true);
      Previews[name] = new CompositeSprite(Vector2.Zero, tempComposite.Entities);
      tempComposite.Dispose();
    }
  }

  public static async Task SaveAndRegisterComposite(CompositeController composite, string blueprintName)
  {
    var entities = composite.Entities.Cast<WorldEntity>().ToList();
    var blueprint = BlueprintFactory.CreateBlueprint(entities, blueprintName);
    await BlueprintFactory.SaveBlueprintAsync(blueprint);
    Previews[blueprintName] = new CompositeSprite(composite.Position.Value, composite.Entities);
  }

  public static void DeleteBlueprint(string blueprintName)
  {
    BlueprintFactory.DeleteBlueprintAsync(blueprintName).GetAwaiter().GetResult();
    Previews.Remove(blueprintName);
  }

  private static CompositeBlueprint CreateSingleEntityBlueprint(ID_ENTITY id)
  {
    return new CompositeBlueprint
    {
      Name = id.ToString(),
      Entities = new List<EntityPlacement> { new EntityPlacement { Id = 0, EntityType = id } },
      Connections = new List<Connection>()
    };
  }

  private static CompositeBlueprint CreateCrossShapeBlueprint()
  {
    return new CompositeBlueprint
    {
      Name = DEFAULT_CROSS,
      Entities = new List<EntityPlacement>
      {
          new EntityPlacement { Id = 0, EntityType = ID_ENTITY.HULL_RECTANGULAR_BAD },
          new EntityPlacement { Id = 1, EntityType = ID_ENTITY.HULL_RECTANGULAR_BAD },
          new EntityPlacement { Id = 2, EntityType = ID_ENTITY.HULL_RECTANGULAR_BAD },
          new EntityPlacement { Id = 3, EntityType = ID_ENTITY.HULL_RECTANGULAR_BAD },
          new EntityPlacement { Id = 4, EntityType = ID_ENTITY.HULL_RECTANGULAR_BAD }
      },
      Connections = new List<Connection>
      {
          new Connection { EntityId1 = 0, EntityId2 = 1, LinkIndex1 = 0, LinkIndex2 = 2 },
          new Connection { EntityId1 = 0, EntityId2 = 2, LinkIndex1 = 1, LinkIndex2 = 2 },
          new Connection { EntityId1 = 0, EntityId2 = 3, LinkIndex1 = 2, LinkIndex2 = 2 },
          new Connection { EntityId1 = 0, EntityId2 = 4, LinkIndex1 = 3, LinkIndex2 = 2 }
      }
    };
  }
}