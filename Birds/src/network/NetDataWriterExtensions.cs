using Birds.src.api.contracts;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace Birds.src.network;

public static class NetDataWriterExtensions
{
  public static void PutVector2(this NetDataWriter writer, Vector2 vector)
  {
    writer.Put(vector.X);
    writer.Put(vector.Y);
  }

  public static void PutInputMessage(this NetDataWriter writer, InputMessage input)
  {
    writer.Put(input.PlayerId);
    writer.Put(input.Tick);
    writer.Put(input.IsPressed);
    writer.PutVector2(input.PositionGameCoords);
    writer.PutVector2(input.CameraPosition);
    writer.Put(input.CameraZoom);
  }

  public static void PutPlayerJoinRequest(this NetDataWriter writer, PlayerJoinRequest request)
  {
    writer.Put(request.DisplayName);
  }

  public static void PutGameStateMessage(this NetDataWriter writer, GameStateMessage state)
  {
    writer.Put(state.Tick);
    writer.Put(state.PlayerId);
    writer.Put(state.EntityUpdatesPerPlayer.Count);

    foreach (var update in state.EntityUpdatesPerPlayer.Values)
    {
      writer.Put(update.EntityId);

      writer.Put(update.Position.HasValue);
      if (update.Position.HasValue)
        writer.PutVector2(update.Position.Value);

      writer.Put(update.Velocity.HasValue);
      if (update.Velocity.HasValue)
        writer.PutVector2(update.Velocity.Value);

      writer.Put(update.Rotation.HasValue);
      if (update.Rotation.HasValue)
        writer.Put(update.Rotation.Value);
    }
  }

  public static void PutEntitySpawnData(this NetDataWriter writer, EntitySpawnData entity)
  {
    writer.Put(entity.Id);
    writer.Put((int)entity.EntityType);
    writer.PutVector2(entity.Position);
    writer.PutVector2(entity.Velocity);
    writer.Put(entity.Rotation);
  }

  public static void PutCompositeSpawnData(this NetDataWriter writer, CompositeSpawnData composite)
  {
    writer.Put(composite.Id);
    writer.PutVector2(composite.SpawnPosition);

    writer.Put(composite.Entities.Count);
    foreach (var e in composite.Entities)
    {
      writer.Put(e.Id);
      writer.Put((int)e.EntityType);
    }

    writer.Put(composite.Connections.Count);
    foreach (var conn in composite.Connections)
    {
      writer.Put(conn.EntityId1);
      writer.Put(conn.EntityId2);
      writer.Put(conn.LinkIndex1);
      writer.Put(conn.LinkIndex2);
    }

    writer.Put(composite.ServerEntityIdByBlueprintIndex.Count);
    foreach (var kvp in composite.ServerEntityIdByBlueprintIndex)
    {
      writer.Put(kvp.Key);
      writer.Put(kvp.Value);
    }
  }

  public static void PutControllerSpawnMessage(this NetDataWriter writer, ControllerSpawnMessage message)
  {
    writer.Put(message.Id);
    writer.Put(message.OwnerId != null);
    if (message.OwnerId != null)
      writer.Put(message.OwnerId);
    writer.Put((int)message.ControllerType);
    writer.PutVector2(message.Position);

    writer.Put(message.DirectEntities.Count);
    foreach (var e in message.DirectEntities)
      writer.PutEntitySpawnData(e);

    writer.Put(message.Composites.Count);
    foreach (var c in message.Composites)
      writer.PutCompositeSpawnData(c);
  }

  public static void PutWorldSnapshotMessage(this NetDataWriter writer, WorldSnapshotMessage snapshot)
  {
    writer.PutControllerSpawnMessage(snapshot.OwnController);

    writer.Put(snapshot.Controllers.Count);
    foreach (var c in snapshot.Controllers)
      writer.PutControllerSpawnMessage(c);
  }
}
