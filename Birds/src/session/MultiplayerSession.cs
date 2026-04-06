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

  public override async Task Initialize()
  {
    var spawnReceived = new TaskCompletionSource<bool>();

    void OnSpawn(ControllerSpawnMessage msg)
    {
      spawnReceived.TrySetResult(true);
    }

    _networkTransport.ControllerSpawnReceived += OnSpawn;

    await Connect();
    SendPlayerJoin();

    await spawnReceived.Task;

    _networkTransport.ControllerSpawnReceived -= OnSpawn;

    _worldRenderer = new WorldRenderer(world, LocalPlayer.Camera);
    _isInitialized = true;
  }


  public override async Task Connect()
  {
    await _networkTransport.Connect();
    _networkTransport.StateReceived += OnGameStateReceived;
    _networkTransport.ControllerSpawnReceived += OnControllerSpawned;
  }

  private void SendPlayerJoin()
  {
    _networkTransport.SendPlayerJoin(new PlayerJoinRequest
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
      var entity = WorldEntityFactory.GetEntity(entityData.Position, entityData.EntityType);
      entity.Rotation.Value = entityData.Rotation;
      entity.Id = entityData.Id;
      allEntities.Add(entity);
    }

    foreach (var compositeData in msg.Composites)
    {
      var entities = NetworkMessageFactory.CreateEntitiesFromSpawnData(compositeData);
      var composite = CompositeControllerFactory.GetComposite(entities.Cast<IEntity>().ToList());
      composite.Id = compositeData.Id;
      composite.Position.Value = compositeData.SpawnPosition;
      allEntities.Add(composite);
    }

    bool isLocalPlayer = msg.ControllerType == ID_CONTROLLER.PLAYER
                         && !world.Controllers.Any();

    Controller controller = ControllerFactory.Create(allEntities, msg.ControllerType,
        isLocalPlayer
        ? input
        : null);
    controller.Id = msg.Id;
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

  private void OnGameStateReceived(GameStateMessage gameState)
  {
    if (!_isInitialized) return;
    ApplyGameStateUpdates(gameState);
  }

  private void ApplyGameStateUpdates(GameStateMessage gameState)
  {
    foreach (var update in gameState.EntityUpdatesPerPlayer.Values)
    {
      var entity = world.Controllers
          .FlattenControllerHierarchy()
          .OfType<WorldEntity>()
          .FirstOrDefault(e => e.Id == update.EntityId);

      if (entity == null) continue;

      if (update.Position.HasValue)
        entity.Position.Value = update.Position.Value;

      if (update.Velocity.HasValue)
        entity.Velocity.Value = update.Velocity.Value;

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

  private void SendInputToServer(GameTime gameTime)
  {
    _networkTransport.SendInput(new InputMessage
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

  public override async Task Disconnect()
  {
    _networkTransport.StateReceived -= OnGameStateReceived;
    _networkTransport.ControllerSpawnReceived -= OnControllerSpawned;
    await _networkTransport.Disconnect();
  }
}
