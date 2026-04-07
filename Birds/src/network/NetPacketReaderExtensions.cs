using Birds.src.api.contracts;
using Birds.src.containers.composite.blueprints.parts;
using Birds.src.utility;
using LiteNetLib;
using Microsoft.Xna.Framework;

namespace Birds.src.network;

public static class NetPacketReaderExtensions
{
  public static Vector2 GetVector2(this NetPacketReader reader)
  {
    return new Vector2(reader.GetFloat(), reader.GetFloat());
  }

  public static InputMessage GetInputMessage(this NetPacketReader reader)
  {
    return new InputMessage
    {
      PlayerId = reader.GetString(),
      Tick = reader.GetLong(),
      IsPressed = reader.GetBool(),
      PositionGameCoords = reader.GetVector2(),
      CameraPosition = reader.GetVector2(),
      CameraZoom = reader.GetFloat()
    };
  }

  public static PlayerJoinRequest GetPlayerJoinRequest(this NetPacketReader reader)
  {
    return new PlayerJoinRequest
    {
      PlayerId = reader.GetString(),
      DisplayName = reader.GetString()
    };
  }

  public static GameStateMessage GetGameStateMessage(this NetPacketReader reader)
  {
    var gameState = new GameStateMessage
    {
      Tick = reader.GetLong(),
      PlayerId = reader.GetString()
    };

    int count = reader.GetInt();
    for (int i = 0; i < count; i++)
    {
      var entityId = reader.GetString();
      var update = new EntityStateUpdate { EntityId = entityId };

      if (reader.GetBool())
        update.Position = reader.GetVector2();

      if (reader.GetBool())
        update.Velocity = reader.GetVector2();

      if (reader.GetBool())
        update.Rotation = reader.GetFloat();

      gameState.EntityUpdatesPerPlayer[entityId] = update;
    }

    return gameState;
  }

  public static EntitySpawnData GetEntitySpawnData(this NetPacketReader reader)
  {
    return new EntitySpawnData
    {
      Id = reader.GetString(),
      EntityType = (ID_ENTITY)reader.GetInt(),
      Position = reader.GetVector2(),
      Velocity = reader.GetVector2(),
      Rotation = reader.GetFloat()
    };
  }

  public static CompositeSpawnData GetCompositeSpawnData(this NetPacketReader reader)
  {
    var composite = new CompositeSpawnData
    {
      Id = reader.GetString(),
      SpawnPosition = reader.GetVector2()
    };

    int entityCount = reader.GetInt();
    for (int i = 0; i < entityCount; i++)
    {
      composite.Entities.Add(new EntityPlacement
      {
        Id = reader.GetInt(),
        EntityType = (ID_ENTITY)reader.GetInt()
      });
    }

    int connCount = reader.GetInt();
    for (int i = 0; i < connCount; i++)
    {
      composite.Connections.Add(new Connection
      {
        EntityId1 = reader.GetInt(),
        EntityId2 = reader.GetInt(),
        LinkIndex1 = reader.GetInt(),
        LinkIndex2 = reader.GetInt()
      });
    }

    int mappingCount = reader.GetInt();
    for (int i = 0; i < mappingCount; i++)
    {
      int blueprintId = reader.GetInt();
      string serverId = reader.GetString();
      composite.ServerEntityIdByBlueprintIndex[blueprintId] = serverId;
    }

    return composite;
  }

  public static ControllerSpawnMessage GetControllerSpawnMessage(this NetPacketReader reader)
  {
    var message = new ControllerSpawnMessage
    {
      Id = reader.GetString(),
      OwnerId = reader.GetBool() ? reader.GetString() : null,
      ControllerType = (ID_CONTROLLER)reader.GetInt(),
      Position = reader.GetVector2()
    };

    int directCount = reader.GetInt();
    for (int i = 0; i < directCount; i++)
      message.DirectEntities.Add(reader.GetEntitySpawnData());

    int compositeCount = reader.GetInt();
    for (int i = 0; i < compositeCount; i++)
      message.Composites.Add(reader.GetCompositeSpawnData());

    return message;
  }

  public static WorldSnapshotMessage GetWorldSnapshotMessage(this NetPacketReader reader)
  {
    var snapshot = new WorldSnapshotMessage
    {
      OwnController = reader.GetControllerSpawnMessage()
    };

    int count = reader.GetInt();
    for (int i = 0; i < count; i++)
      snapshot.Controllers.Add(reader.GetControllerSpawnMessage());

    return snapshot;
  }
}
