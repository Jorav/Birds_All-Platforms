using Birds.src.api.client;
using Birds.src.api.contracts;
using Birds.src.api.network;
using Birds.src.api.transport;
using Birds.src.containers.composite;
using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.player;
using Birds.src.session;
using Birds.src.session.world;
using Birds.src.utility;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birds.src.session;

public class MultiplayerSession(
    ClientSession session,
    Input input,
    Game1 game,
    GraphicsDevice graphicsDevice,
    IClientNetworkTransport networkTransport,
    bool isHost = false) : GameSession(session, input)
{
  private readonly Game1 _game = game;
  private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
  private readonly IClientNetworkTransport _networkTransport = networkTransport;
  private readonly bool _isHost = isHost;
  private readonly string _localPlayerId = Guid.NewGuid().ToString();

  private WorldRenderer _worldRenderer;
  private bool _isInitialized = false;
  private Dictionary<string, WorldEntity> _entityRegistry = new();

  public override async Task InitializeAsync()
  {
    await ConnectAsync();
    await SendPlayerJoinAsync();
    await Task.Delay(1000);
    _worldRenderer = new WorldRenderer(world, LocalPlayer.Camera);
    _isInitialized = true;
  }

  public override async Task ConnectAsync()
  {
    await _networkTransport.ConnectAsync();
    _networkTransport.StateReceived += OnGameStateReceived;
    _networkTransport.ControllerSpawnReceived += OnControllerSpawned;
  }

  private async Task SendPlayerJoinAsync()
  {
    await _networkTransport.SendPlayerJoinAsync(new PlayerJoinRequest
    {
      PlayerId = _localPlayerId,
      DisplayName = ClientSession.Current.DisplayName
    });
  }

  private void OnControllerSpawned(ControllerSpawnMessage msg)
  {
    var allEntities = new List<IEntity>();

    foreach (var entityData in msg.DirectEntities)
    {
      var entity = BuildAndRegisterWorldEntity(entityData);
      allEntities.Add(entity);
    }

    foreach (var compositeData in msg.Composites)
    {
      var entities = NetworkMessageFactory.CreateEntitiesFromSpawnData(compositeData);

      for (int i = 0; i < entities.Count; i++)
      {
        if (compositeData.ServerEntityIdByBlueprintIndex.TryGetValue(i, out string serverEntityId))
        {
          _entityRegistry[serverEntityId] = entities[i];
        }
      }

      var composite = new CompositeController();
      composite.Entities.Set(entities.Cast<IEntity>().ToList());
      CompositeControllerFactory.SetCompositeModules(composite, ID_COMPOSITE.DEFAULT);
      composite.Position.Value = compositeData.SpawnPosition;

      allEntities.Add(composite);
    }

    bool isLocalPlayer = msg.ControllerType == ID_CONTROLLER.PLAYER
                         && !world.Controllers.Any();

    Controller controller = ControllerFactory.Create(allEntities, msg.ControllerType,
        isLocalPlayer ? input : null);
    controller.Position.Value = msg.Position;

    if (isLocalPlayer)
    {
      var player = new Player(_localPlayerId, input);
      player.SetController(controller);
      AddPlayer(player);
    }
    else
    {
      switch (msg.ControllerType)
      {
        case ID_CONTROLLER.BACKGROUND_SUN:
          world.Backgrounds.Add((Background)controller);
          break;
        case ID_CONTROLLER.FOREGROUND_CLOUD:
          world.Foregrounds.Add((Background)controller);
          break;
        default:
          world.AddController(controller);
          break;
      }
    }
  }

  private WorldEntity BuildAndRegisterWorldEntity(EntitySpawnData data)
  {
    var entity = WorldEntityFactory.GetEntity(data.Position, data.EntityType);
    entity.Rotation.Value = data.Rotation;
    _entityRegistry[data.EntityId] = entity;
    return entity;
  }

  private void OnGameStateReceived(GameStateMessage gameState)
  {
    if (!_isInitialized) return;
    ApplyGameStateUpdates(gameState);
  }

  private void ApplyGameStateUpdates(GameStateMessage gameState)
  {
    foreach (var update in gameState.EntityUpdatesPerPlayer.Values)
    {
      if (!_entityRegistry.TryGetValue(update.EntityId, out var entity))
        continue;

      if (update.X.HasValue && update.Y.HasValue)
        entity.Position.Value = new Vector2(update.X.Value, update.Y.Value);

      if (update.VelX.HasValue && update.VelY.HasValue)
        entity.Velocity.Value = new Vector2(update.VelX.Value, update.VelY.Value);

      if (update.Rotation.HasValue)
        entity.Rotation.Value = update.Rotation.Value;
    }
  }

  public override void Update(GameTime gameTime)
  {
    if (!_isInitialized) return;

    _networkTransport.PollEvents();
    world.Update(gameTime);
    SendInputToServer(gameTime);
  }

  private async void SendInputToServer(GameTime gameTime)
  {
    await _networkTransport.SendInputAsync(new InputMessage
    {
      PlayerId = _localPlayerId,
      Tick = (long)(gameTime.TotalGameTime.TotalSeconds * 20),
      IsPressed = input.IsPressed,
      PositionGameCoords = input.PositionGameCoords,
      CameraPosition = LocalPlayer?.Camera?.Position ?? Vector2.Zero,
      CameraZoom = LocalPlayer?.Camera?.Zoom ?? 1f
    });
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    if (!_isInitialized) return;
    _graphicsDevice.Clear(Color.CornflowerBlue);
    _worldRenderer.Draw(spriteBatch);
  }

  public override async Task DisconnectAsync()
  {
    _networkTransport.StateReceived -= OnGameStateReceived;
    _networkTransport.ControllerSpawnReceived -= OnControllerSpawned;
    await _networkTransport.DisconnectAsync();
  }
}
