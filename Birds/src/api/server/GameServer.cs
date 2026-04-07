using Birds.src.api.contracts;
using Birds.src.api.network;
using Birds.src.api.transport;
using Birds.src.containers.composite;
using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.player;
using Birds.src.session.world;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Birds.src.api.server;

public class GameServer
{
  private readonly World _world;
  private readonly IServerNetworkTransport _networkTransport;
  private long _currentTick = 0;
  private Dictionary<string, InputMessage> _latestInputs = new();
  private Dictionary<string, Player> _players = new();

  public GameServer(IServerNetworkTransport networkTransport)
  {
    _networkTransport = networkTransport;
    _world = new World();

    _networkTransport.InputReceived += OnInputReceived;
    _networkTransport.PlayerConnected += OnPlayerConnected;
    _networkTransport.PlayerDisconnected += OnPlayerDisconnected;
    _networkTransport.PlayerJoinRequested += OnPlayerJoinRequested;

    WorldInitializer.Initialize(_world);
  }

  private void OnPlayerJoinRequested(PlayerJoinRequest joinRequest)
  {
    string playerId = joinRequest.PlayerId;
    Debug.WriteLine($"[Server] OnPlayerJoinRequested: {playerId}");

    if (_players.ContainsKey(playerId))
    {
      Debug.WriteLine($"[Server] Player {playerId} already exists, ignoring");
      return;
    }

    var networkInput = new NetworkInputState();
    var player = new Player(playerId, networkInput);
    var playerController = _world.AddPlayer(networkInput);
    player.SetController(playerController);
    _players[playerId] = player;

    var snapshot = BuildWorldSnapshot(playerId, playerController);
    Debug.WriteLine($"[Server] Sending world snapshot to {playerId} ({snapshot.Controllers.Count} controllers)");
    _networkTransport.SendWorldSnapshot(snapshot, playerId);

    var newPlayerSpawnMsg = BuildSpawnMessage(playerController);
    foreach (var otherPlayerId in _players.Keys.Where(id => id != playerId))
    {
      Debug.WriteLine($"[Server] Notifying {otherPlayerId} of new player controller {playerController.Id}");
      _networkTransport.SendControllerSpawn(newPlayerSpawnMsg, otherPlayerId);
    }
  }

  private WorldSnapshotMessage BuildWorldSnapshot(string playerId, Controller playerController)
  {
    var ownSpawn = BuildSpawnMessage(playerController);
    ownSpawn.OwnerId = playerId;

    var snapshot = new WorldSnapshotMessage
    {
      OwnController = ownSpawn
    };

    foreach (var c in _world.Controllers.Where(c => c.Id != playerController.Id))
      snapshot.Controllers.Add(BuildSpawnMessage(c));

    return snapshot;
  }

  private ControllerSpawnMessage BuildSpawnMessage(Controller controller)
  {
    var message = new ControllerSpawnMessage
    {
      Id = controller.Id,
      ControllerType = controller.ControllerId,
      Position = controller.Position.Value
    };

    foreach (var entity in controller.Entities)
    {
      if (entity is CompositeController composite)
      {
        message.Composites.Add(NetworkMessageFactory.CreateCompositeSpawnData(composite));
      }
      else if (entity is WorldEntity worldEntity)
      {
        message.DirectEntities.Add(new EntitySpawnData
        {
          Id = worldEntity.Id,
          EntityType = worldEntity.EntityID,
          Position = worldEntity.Position.Value,
          Velocity = worldEntity.Velocity.Value,
          Rotation = worldEntity.Rotation.Value
        });
      }
    }

    return message;
  }

  private void OnInputReceived(InputMessage input)
  {
    _latestInputs[input.PlayerId] = input;
  }

  private void OnPlayerConnected(string playerId)
  {
    Debug.WriteLine($"[Server] Player connected: {playerId}");
  }

  private void OnPlayerDisconnected(string playerId)
  {
    Debug.WriteLine($"[Server] Player disconnected: {playerId}");
    if (_players.TryGetValue(playerId, out var player))
    {
      _world.RemovePlayer(player.Controller);
      _players.Remove(playerId);
    }
  }

  public void Update(GameTime gameTime)
  {
    ApplyPlayerInputs();
    _world.Update(gameTime);
    SendGameStateUpdates();
    _currentTick++;
  }

  private void ApplyPlayerInputs()
  {
    foreach (var kvp in _latestInputs)
    {
      if (_players.TryGetValue(kvp.Key, out var player) &&
          player.InputState is NetworkInputState networkInput)
        networkInput.ApplyInput(kvp.Value);
    }
    _latestInputs.Clear();
  }

  private void SendGameStateUpdates()
  {
    var allEntities = _world.Controllers
        .FlattenControllerHierarchy()
        .OfType<WorldEntity>()
        .Where(e => e.Position.IsDirty || e.Velocity.IsDirty || e.Rotation.IsDirty)
        .ToList();

    foreach (var playerId in _players.Keys)
    {
      var message = new GameStateMessage { Tick = _currentTick, PlayerId = playerId };

      foreach (var entity in allEntities)
      {
        var update = new EntityStateUpdate { EntityId = entity.Id };

        if (entity.Position.IsDirty)
          update.Position = entity.Position.Value;

        if (entity.Velocity.IsDirty)
          update.Velocity = entity.Velocity.Value;

        if (entity.Rotation.IsDirty)
          update.Rotation = entity.Rotation.Value;

        message.EntityUpdatesPerPlayer[entity.Id] = update;
      }

      if (message.EntityUpdatesPerPlayer.Count > 0)
        _networkTransport.SendGameState(message, playerId);
    }

    foreach (var entity in allEntities)
    {
      entity.Position.ClearDirty();
      entity.Velocity.ClearDirty();
      entity.Rotation.ClearDirty();
    }
  }
}
