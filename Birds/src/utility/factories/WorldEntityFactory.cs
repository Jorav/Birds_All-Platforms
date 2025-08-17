using Microsoft.Xna.Framework;
using Birds.src.utility;
using System;
using System.Collections.Generic;
using Birds.src.entities;
using Birds.src.menu;
using Birds.src.modules.shared.bounding_area;
using Birds.src.modules.entity;

namespace Birds.src.factories;
public static class EntityFactory
{
  public static Stack<WorldEntity> availableEntities = new();

  public static WorldEntity GetEntity(Vector2 position, ID_ENTITY id)
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
    SetModules(we, id);
    we.Position.Value = position;
    we.AddLinks();
    return we;
  }

  private static void SetDefaultAttributes(WorldEntity we, ID_ENTITY id, Vector2 position)
  {
    float rotation = 0;
    float mass = 1;
    float thrust = 1;
    float friction = 0.1f;
    bool isVisible = true;
    bool isCollidable = true;
    float scale = 1;
  }

  public static void SetModules(WorldEntity we, ID_ENTITY id)
  {
    switch (id)
    {
      case ID_ENTITY.DEFAULT:
        we.AddModule(new SpriteModule(id));
        we.AddModule(new EntityMovementModule());
        we.AddModule(new BCCollisionDetectionModule());
        we.AddModule(new OBBCollisionDetectionModule());
        we.AddModule(new EntityCollisionHandlerModule());
        break;

      /*
      case IDs.EMPTY_LINK: return new RectangularComposite(new Sprite(emptyLink), position);
      case IDs.COMPOSITE: return new RectangularComposite(new Sprite(rectangularHull), position) { Mass = 2 };
      case IDs.CIRCULAR_COMPOSITE: return new CircularComposite(new Sprite(circularHull), position) { Mass = 2, Scale = 2 };
      case IDs.LINK_COMPOSITE: return new LinkComposite(new Sprite(linkHull), position) { Mass = 1f, Thrust = 0.5f };
      case IDs.TRIANGULAR_EQUAL_COMPOSITE: return new TriangularEqualLeggedComposite(new Sprite(triangularEqualLeggedHull), position) { Mass = 2 };
      case IDs.TRIANGULAR_90ANGLE_COMPOSITE: return new Triangular90AngleComposite(new Sprite(triangular90AngleHull), position) { Mass = 2 };
      case IDs.SHOOTER: return new Shooter(new Sprite(gun), position, (Projectile)Create(position, IDs.PROJECTILE))
      { Thrust = 0, FireRatePerSecond = 10f, FiringStrength = 14, Mass = 0.5f };
      case IDs.PROJECTILE: return new Projectile(new Sprite(projectile), position)
      { Mass = 0.4f, Friction = 0.03f, MaxLifeSpan = 3f, MinLifeSpan = 1f };
      case IDs.SPIKE: return new Spike(new Sprite(spike), position) { Thrust = 0, Mass = 0.5f };
      case IDs.ENGINE: return new WorldEntity(new Sprite(engine), position) {Mass = 0.5f, Thrust = 2f };
      //case (int)IDs.COMPOSITE: return new Composite(new Sprite(hull), position);*/
      #region background
      case ID_ENTITY.CLOUD:
        we.AddModule(new EntityMovementModule());
        break;

      case ID_ENTITY.SUN:
        we.AddModule(new EntityMovementModule());
        break;
      #endregion

      default:
        throw new NotImplementedException();
    }
  }

  public static List<IEntity> CreateEntities(Vector2 position, int numberOfEntities, ID_ENTITY id, bool isBackground = false)
  {
    List<IEntity> returnedList = new List<IEntity>();
    if (numberOfEntities == 1)
    {
      WorldEntity we = EntityFactory.GetEntity(position, id);
      returnedList.Add(we);
    }
    else if (numberOfEntities > 1)
    {
      Random rnd = new Random();
      if (!isBackground)
      {
        for (int i = 0; i < numberOfEntities; i++)
        {
          float rRadius = (float)(rnd.NextDouble() * 5 * numberOfEntities);
          float rAngle = (float)(rnd.NextDouble() * 2 * Math.PI);
          WorldEntity we = EntityFactory.GetEntity(new Vector2((float)Math.Sin(rAngle), (float)Math.Cos(rAngle)) * rRadius + position, id);
          returnedList.Add(we);
        }
      }
      else
      {
        for (int i = 0; i < numberOfEntities; i++)
        {
          float x = GameState.Player.Position.Value.X + (float)((rnd.NextDouble() * (Game1.ScreenWidth - 32 * 2) - Game1.ScreenWidth / 2) + 32);
          float y = GameState.Player.Position.Value.Y + (float)((rnd.NextDouble() * (Game1.ScreenHeight - 32 * 2) - Game1.ScreenHeight / 2) + 32);
          WorldEntity we = EntityFactory.GetEntity(new Vector2(x, y), id);
          returnedList.Add(we);
        }
      }
    }
    return returnedList;
  }

  /**
  public static void LoadTextures(Texture2D hull, Texture2D gun, Texture2D projectile, Texture2D cloud) //TODO - add support for multiple skins
  {
      this.hull = hull;
      this.gun = gun;
      this.projectile = projectile;
      this.cloud = cloud;
  }*/
}
