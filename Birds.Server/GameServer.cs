using Birds.src.api.contracts;
using Birds.src.containers.controller;
using Birds.src.events;
using Birds.src.player;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birds.server;

public class GameServer
{
  private readonly GameController _gameController;
  private long _currentTick = 0;
  private Dictionary<string, InputMessage> _latestInputs = new();
  private Dictionary<string, Player> _players = new();

  public event Action<GameStateMessage> StateChanged;

  public GameServer()
  {
    _gameController = new GameController();
    _players = new Dictionary<string, Player>();
  }

  public void ReceiveInput(InputMessage input)
  {
    _latestInputs[input.PlayerId] = input;
  }

  public void Update(GameTime gameTime)
  {
    _gameController.Update(gameTime);

    foreach (var kvp in _players)
    {
      var playerId = kvp.Key;
      var player = kvp.Value;

      if (_latestInputs.TryGetValue(playerId, out var input))
      {
        var stateUpdate = CollectDirtyStateForPlayer(playerId, player.Camera, input.CameraZoom);
        if (stateUpdate.EntityUpdatesPerPlayer.Count > 0)
        {
          StateChanged?.Invoke(stateUpdate);
        }
      }
    }

    _currentTick++;
  }

  private GameStateMessage CollectDirtyStateForPlayer(string playerId, Camera camera, float cameraZoom)
  {
    var message = new GameStateMessage
    {
      Tick = _currentTick,
      PlayerId = playerId
    };

    var visibleBounds = camera.GetVisibleBounds(cameraZoom, buffer: 500f);

    var allEntities = FlattenControllerHierarchy();

    foreach (var entity in allEntities)
    {
      if (!camera.IsEntityWithinFrame(entity, cameraZoom, buffer: 500f))
        continue;

      var entityId = GetEntityId(entity);

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

  private List<ModuleContainer> FlattenControllerHierarchy()
  {
    var result = new List<ModuleContainer>();
    /**
    foreach (var controller in GameController.controllers)
    {
      result.Add(controller);
      foreach (var child in controller.Entities.OfType<ModuleContainer>())
      {
        result.AddRange(FlattenEntityHierarchy(child));
      }
    }
    */
    return result;
  }

  private List<ModuleContainer> FlattenEntityHierarchy(ModuleContainer container)
  {
    var result = new List<ModuleContainer> { container };

    foreach (var child in container.Entities.OfType<ModuleContainer>())
    {
      result.AddRange(FlattenEntityHierarchy(child));
    }

    return result;
  }

  private string GetEntityId(ModuleContainer entity)
  {
    // TODO: Add proper ID system to ModuleContainer
    return entity.GetHashCode().ToString();
  }
}
