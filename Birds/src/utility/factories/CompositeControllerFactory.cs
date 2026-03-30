using Microsoft.Xna.Framework;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using Birds.src.storage;
using Birds.src.storage.implementations;
using System.Linq;
using Birds.src.containers.composite.blueprints;
using Birds.src.containers.composite.blueprints.parts;
using Birds.src.containers.entity;
using Birds.src.containers.composite;
using Birds.src.modules.controller;
using Birds.src.modules.composite;
using Birds.src.modules.entity.collision_handling;
using Birds.src.events;
using Birds.src.modules.shared.collision_detection;
using Birds.src.collision.BVH;
using Birds.src.visual;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Birds.src.factories;

public static class CompositeControllerFactory
{
  public static Stack<CompositeController> availableEntities = new(100);
  public const string DEFAULT_SINGLE = "Single Entity";
  public const string DEFAULT_CROSS = "Cross Shape";
  public static Dictionary<string, ISprite> Previews { get; set; } = new();

  public static CompositeController GetComposite(Vector2 position, string blueprintName, bool useGeometricCenter = false)
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
    var entities = BlueprintFactory.CreateFromBlueprint(blueprint, position, useGeometricCenter);
    var iEntities = entities.Cast<IEntity>().ToList();

    compositeController.Position.Value = position;
    compositeController.Entities.Set(iEntities);
    SetCompositeModules(compositeController, GetCompositeIdFromBlueprint(blueprintName));

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
        composite = GetComposite(compositePosition, id);
        returnedList.Add(composite);
      }
    }
    return returnedList;
  }

  private static void SetCompositeModules(CompositeController composite, ID_COMPOSITE id)
  {
    composite.ClearModules();
    switch (id)
    {
      default:
        composite.AddModule(new SubEntityVelocityReseter());
        composite.AddModule(new CohesiveGroupRotationModule());
        composite.AddModule(new CompositeMovementModule());
        composite.AddModule(new LinkManagementModule());
        composite.AddModule(new GroupMassModule());
        composite.AddModule(new GroupWeightedPositionModule());
        composite.AddModule(new GroupRadiusModule());
        composite.AddModule(new GroupThrustModule());
        composite.AddModule(new GroupCollisionDetectionModule(new ListCollisionStructure(), evaluateInternalCollisions: false));
        composite.AddModule(new SubEntityCollisionExtractionModule());
        composite.AddModule(GetCollisionHandler());
        composite.AddModule(new GroupDrawModule());
        break;
    }
  }

  public static CollisionHandlerModule GetCollisionHandler()
  {
    var collisionHandler = new CollisionHandlerModule();
    collisionHandler.AddResponse(new MomentumTransfer());
    collisionHandler.AddResponse(new OverlapRepulsion());
    return collisionHandler;
  }

  private static ID_COMPOSITE GetCompositeIdFromBlueprint(string blueprintName)
  {
    return ID_COMPOSITE.DEFAULT;
  }

  private static CompositeBlueprint GetBlueprintByName(string blueprintName)
  {
    var premadeBlueprint = TryGetPremadeBlueprint(blueprintName);
    if (premadeBlueprint != null) return premadeBlueprint;

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
    switch (blueprintName)
    {
      case DEFAULT_SINGLE: return CreateSingleEntityBlueprint();
      case DEFAULT_CROSS: return CreateCrossShapeBlueprint();
      default: return null;
    }
  }

  public static void InitializePreviews()
  {
    var savedNames = BlueprintFactory.GetBlueprintNamesAsync().GetAwaiter().GetResult();
    var blueprintNames = new List<string> { DEFAULT_SINGLE, DEFAULT_CROSS };
    foreach (var name in savedNames)
    {
      if (!blueprintNames.Contains(name))
        blueprintNames.Add(name);
    }

    Vector2 spawnPos = Vector2.Zero;
    foreach (var name in blueprintNames)
    {
      var tempComposite = GetComposite(spawnPos, name, true);
      ISprite preview = new CompositeSprite(spawnPos, tempComposite.Entities);
      Previews[name] = preview;
      tempComposite.Dispose();
    }
  }

  public static async Task SaveAndRegisterComposite(CompositeController composite, string blueprintName)
  {
    var entities = composite.Entities.Cast<WorldEntity>().ToList();
    var blueprint = BlueprintFactory.CreateBlueprint(entities, blueprintName);
    await BlueprintFactory.SaveBlueprintAsync(blueprint);
    ISprite preview = new CompositeSprite(composite.Position.Value, composite.Entities);
    Previews[blueprintName] = preview;
  }

  public static void DeleteBlueprint(string blueprintName)
  {
    BlueprintFactory.DeleteBlueprintAsync(blueprintName).GetAwaiter().GetResult();
    if (Previews.ContainsKey(blueprintName))
    {
      Previews.Remove(blueprintName);
    }
  }

  private static CompositeBlueprint CreateSingleEntityBlueprint()
  {
    return new CompositeBlueprint
    {
      Name = DEFAULT_SINGLE,
      Entities = new List<EntityPlacement> { new EntityPlacement { Id = 0, EntityType = ID_ENTITY.DEFAULT } },
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
          new EntityPlacement { Id = 0, EntityType = ID_ENTITY.DEFAULT }, // Center
          new EntityPlacement { Id = 1, EntityType = ID_ENTITY.DEFAULT }, // Right
          new EntityPlacement { Id = 2, EntityType = ID_ENTITY.DEFAULT }, // Bottom
          new EntityPlacement { Id = 3, EntityType = ID_ENTITY.DEFAULT }, // Left
          new EntityPlacement { Id = 4, EntityType = ID_ENTITY.DEFAULT }  // Top
      },
      Connections = new List<Connection>
      {
          new Connection { EntityId1 = 0, EntityId2 = 1, LinkIndex1 = 0, LinkIndex2 = 2 }, // Center right → Right left
          new Connection { EntityId1 = 0, EntityId2 = 2, LinkIndex1 = 1, LinkIndex2 = 2 }, // Center bottom → Bottom top
          new Connection { EntityId1 = 0, EntityId2 = 3, LinkIndex1 = 2, LinkIndex2 = 2 }, // Center left → Left right
          new Connection { EntityId1 = 0, EntityId2 = 4, LinkIndex1 = 3, LinkIndex2 = 2 }  // Center top → Top bottom
      }
    };
  }
}