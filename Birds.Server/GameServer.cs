using Birds.src.api.contracts;
using Birds.src.api.network;
using Birds.src.api.transport;
using Birds.src.containers.composite;
using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.player;
using Birds.src.session.world;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birds.Server;

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
    if (_players.ContainsKey(playerId)) return;

    var networkInput = new NetworkInputState();
    var player = new Player(playerId, networkInput);
    var controller = _world.AddPlayer(networkInput);
    player.SetController(controller);
    _players[playerId] = player;

    foreach (var c in _world.Controllers)
      _networkTransport.SendControllerSpawn(BuildSpawnMessage(c), playerId);

    SendFullWorldState(playerId);
  }

  private ControllerSpawnMessage BuildSpawnMessage(Controller controller)
  {
    var message = new ControllerSpawnMessage
    {
      ControllerId = controller.GetEntityId(),
      ControllerType = controller.Id,
      Position = controller.Position.Value
    };

    foreach (var entity in controller.Entities)
    {
      if (entity is CompositeController composite)
      {
        var spawnData = NetworkMessageFactory.CreateCompositeSpawnData(composite);
        message.Composites.Add(spawnData);
      }
      else if (entity is WorldEntity worldEntity)
      {
        message.DirectEntities.Add(new EntitySpawnData
        {
          EntityId = worldEntity.GetEntityId(),
          EntityType = worldEntity.EntityID,
          Position = worldEntity.Position.Value,
          Rotation = worldEntity.Rotation.Value
        });
      }
    }

    return message;
  }

  private void SendFullWorldState(string playerId)
  {
    var allEntities = _world.Controllers.FlattenControllerHierarchy()
        .OfType<WorldEntity>();

    var message = new GameStateMessage { Tick = _currentTick, PlayerId = playerId };

    foreach (var entity in allEntities)
    {
      var entityId = entity.GetEntityId();
      message.EntityUpdatesPerPlayer[entityId] = new EntityStateUpdate
      {
        EntityId = entityId,
        X = entity.Position.Value.X,
        Y = entity.Position.Value.Y,
        VelX = entity.Velocity.Value.X,
        VelY = entity.Velocity.Value.Y,
        Rotation = entity.Rotation.Value
      };
    }

    _networkTransport.SendGameState(message, playerId);
  }

  private void OnInputReceived(InputMessage input)
  {
    _latestInputs[input.PlayerId] = input;
  }

  private void OnPlayerConnected(string playerId)
  {
    Console.WriteLine($"Player connected: {playerId}");
  }

  private void OnPlayerDisconnected(string playerId)
  {
    Console.WriteLine($"Player disconnected: {playerId}");
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
        var entityId = entity.GetEntityId();
        var update = new EntityStateUpdate { EntityId = entityId };

        if (entity.Position.IsDirty) { update.X = entity.Position.Value.X; update.Y = entity.Position.Value.Y; }
        if (entity.Velocity.IsDirty) { update.VelX = entity.Velocity.Value.X; update.VelY = entity.Velocity.Value.Y; }
        if (entity.Rotation.IsDirty) update.Rotation = entity.Rotation.Value;

        message.EntityUpdatesPerPlayer[entityId] = update;
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
