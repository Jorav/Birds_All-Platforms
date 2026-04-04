using Microsoft.Xna.Framework;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using Birds.src.menu;
using Birds.src.modules.entity;
using Birds.src.containers.entity;
using Birds.src.modules.entity.collision_handling;
using Birds.src.collision.bounding_areas;
using Birds.src.visual;
using Birds.src.utility.factories;

namespace Birds.src.factories;

public static class WorldEntityFactory
{
  public static Stack<WorldEntity> availableEntities = new(100);

  public static Dictionary<ID_ENTITY, ISprite> Previews { get; private set; } = new();

  public static readonly Dictionary<ID_ENTITY, ID_SPRITE> EntityToSpriteMap = new()
  {
    { ID_ENTITY.HULL_RECTANGULAR_BAD, ID_SPRITE.HULL_RECTANGULAR_BAD },
    { ID_ENTITY.DEFAULT, ID_SPRITE.HULL_RECTANGULAR },
    { ID_ENTITY.HULL_RECTANGULAR_GOOD, ID_SPRITE.HULL_RECTANGULAR_GOOD },
    { ID_ENTITY.CIRCULAR, ID_SPRITE.HULL_CIRCULAR },
    { ID_ENTITY.LINK_COMPOSITE, ID_SPRITE.HULL_LINK },
    { ID_ENTITY.ENGINE_BAD, ID_SPRITE.ENGINE_BAD },
    { ID_ENTITY.ENGINE, ID_SPRITE.ENGINE },
    { ID_ENTITY.ENGINE_GOOD, ID_SPRITE.ENGINE_GOOD },
    { ID_ENTITY.SHOOTER, ID_SPRITE.GUN },
    { ID_ENTITY.SPIKE, ID_SPRITE.SPIKE },
    { ID_ENTITY.FILLER, ID_SPRITE.FILLER },
    { ID_ENTITY.SUN, ID_SPRITE.SUN },
    { ID_ENTITY.CLOUD, ID_SPRITE.CLOUD },
    { ID_ENTITY.HULL_THIN, ID_SPRITE.HULL_THIN},
  };

  static WorldEntityFactory()
  {
    WorldEntityLoader.Initialize();
  }

  public static void InitializePreviews()
  {
    Previews.Clear();
    foreach (var kvp in EntityToSpriteMap)
    {
      var sprite = SpriteFactory.GetSprite(kvp.Value, Vector2.Zero, 1f);
      Previews[kvp.Key] = sprite;
    }
  }

  public static WorldEntity GetEntity(Vector2 position, ID_ENTITY id, bool isComposite = false, ID_SPRITE spriteId = ID_SPRITE.FILLER)
  {
    WorldEntity we;
    if (availableEntities.Count > 0)
    {
      we = availableEntities.Pop();
    }
    else
    {
      we = new WorldEntity();
    }
    
    we.EntityID = id;
    we.Position.Value = position;
    WorldEntityLoader.ApplyConfiguration(we, id, isComposite, spriteId);
    return we;
  }

  public static CollisionHandlerModule GetCollisionHandler(bool isComposite)
  {
    var collisionHandler = new CollisionHandlerModule();
    if (!isComposite)
    {
      collisionHandler.AddResponse(new MomentumTransfer());
      collisionHandler.AddResponse(new OverlapRepulsion());
    }
    return collisionHandler;
  }

  public static List<IEntity> CreateEntities(Vector2 position, int numberOfEntities, ID_ENTITY id, bool isBackground = false, bool isComposite = false)
  {
    List<IEntity> returnedList = new List<IEntity>();
    WorldEntity we = GetEntity(position, id, isComposite);
    returnedList.Add(we);

    if (numberOfEntities > 1)
    {
      Random rnd = new Random();
      if (!isBackground)
      {
        for (int i = 0; i < numberOfEntities - 1; i++)
        {
          float rRadius = (float)(rnd.NextDouble() * we.Radius * 2 * Math.Sqrt(numberOfEntities));
          float rAngle = (float)(rnd.NextDouble() * 2 * Math.PI);
          we = GetEntity(new Vector2((float)Math.Cos(rAngle), (float)Math.Sin(rAngle)) * rRadius + position, id, isComposite);
          returnedList.Add(we);
        }
      }
      else//TODO: Update how backgrounds are handled
      {
        for (int i = 0; i < numberOfEntities - 1; i++)
        {
          float x = 0 + (float)((rnd.NextDouble() * (Game1.ScreenWidth - 32 * 2) - Game1.ScreenWidth / 2) + 32);
          float y = 0 + (float)((rnd.NextDouble() * (Game1.ScreenHeight - 32 * 2) - Game1.ScreenHeight / 2) + 32);
          we = GetEntity(new Vector2(x, y), id, isComposite);
          returnedList.Add(we);
        }
      }
    }
    return returnedList;
  }
}