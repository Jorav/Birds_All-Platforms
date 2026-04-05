using Birds.src.api.contracts;
using Birds.src.api.transport;
using Birds.src.events;
using Birds.src.player;
using Birds.src.session.world;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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
    Console.WriteLine($"Player join request: {playerId}");

    if (!_players.ContainsKey(playerId))
    {
      var player = new Player(playerId, null);
      var playerController = _world.AddPlayer(null);
      player.SetController(playerController);
      _players[playerId] = player;

      SendFullWorldState(playerId);
    }
  }

  private void SendFullWorldState(string playerId)
  {
    var allEntities = _world.Controllers.FlattenControllerHierarchy();
    var stateUpdate = new GameStateMessage
    {
      Tick = _currentTick,
      PlayerId = playerId
    };

    foreach (var entity in allEntities)
    {
      var entityId = ModuleContainerExtensions.GetEntityId(entity);
      var update = new EntityStateUpdate { EntityId = entityId };

      update.X = entity.Position.Value.X;
      update.Y = entity.Position.Value.Y;
      update.VelX = entity.Velocity.Value.X;
      update.VelY = entity.Velocity.Value.Y;
      update.Rotation = entity.Rotation.Value;

      stateUpdate.EntityUpdatesPerPlayer[entityId] = update;
    }

    _networkTransport.SendGameState(stateUpdate, playerId);
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
      var playerId = kvp.Key;
      var input = kvp.Value;

      if (_players.TryGetValue(playerId, out var player))
      {
      }
    }
    _latestInputs.Clear();
  }

  private void SendGameStateUpdates()
  {
    foreach (var playerId in _players.Keys)
    {
      var stateUpdate = CollectDirtyStateForPlayer(playerId);
      if (stateUpdate.EntityUpdatesPerPlayer.Count > 0)
      {
        _networkTransport.SendGameState(stateUpdate, playerId);
      }
    }
  }

  private GameStateMessage CollectDirtyStateForPlayer(string playerId)
  {
    var message = new GameStateMessage
    {
      Tick = _currentTick,
      PlayerId = playerId
    };

    var allEntities = _world.Controllers.FlattenControllerHierarchy();
    foreach (var entity in allEntities)
    {
      var entityId = ModuleContainerExtensions.GetEntityId(entity);
      var update = new EntityStateUpdate { EntityId = entityId };
      bool hasDirtyProps = false;

      if (entity.Position.IsDirty)
      {
        update.X = entity.Position.Value.X;
        update.Y = entity.Position.Value.Y;
        entity.Position.ClearDirty();
        hasDirtyProps = true;
      }

      if (entity.Velocity.IsDirty)
      {
        update.VelX = entity.Velocity.Value.X;
        update.VelY = entity.Velocity.Value.Y;
        entity.Velocity.ClearDirty();
        hasDirtyProps = true;
      }

      if (entity.Rotation.IsDirty)
      {
        update.Rotation = entity.Rotation.Value;
        entity.Rotation.ClearDirty();
        hasDirtyProps = true;
      }

      if (hasDirtyProps)
      {
        message.EntityUpdatesPerPlayer[entityId] = update;
      }
    }

    return message;
  }
}
