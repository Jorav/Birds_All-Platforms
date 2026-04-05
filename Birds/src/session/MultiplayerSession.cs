using Birds.src.api.client;
using Birds.src.api.contracts;
using Birds.src.api.transport;
using Birds.src.events;
using Birds.src.network;
using Birds.src.player;
using Birds.src.session.world;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
  private WorldRenderer _worldRenderer;
  private bool _isInitialized = false;

  public override async Task InitializeAsync()
  {
    await ConnectAsync();
    await SendPlayerJoinAsync();

    await Task.Delay(1000);

    var player = new Player(localPlayerId, input);
    var playerController = world.AddPlayer(input);
    player.SetController(playerController);
    AddPlayer(player);

    _worldRenderer = new WorldRenderer(world, player.Camera);
    _isInitialized = true;
  }

  public override async Task ConnectAsync()
  {
    var transport = (IClientNetworkTransport)networkTransport;
    await transport.ConnectAsync();
    transport.StateReceived += OnGameStateReceived;
  }

  private async Task SendPlayerJoinAsync()
  {
    var transport = (IClientNetworkTransport)networkTransport;
    var joinRequest = new PlayerJoinRequest
    {
      PlayerId = localPlayerId,
      DisplayName = "Player"
    };
    await transport.SendPlayerJoinAsync(joinRequest);
  }

  public override async Task DisconnectAsync()
  {
    var transport = (IClientNetworkTransport)networkTransport;
    transport.StateReceived -= OnGameStateReceived;
    await transport.DisconnectAsync();
  }

  private void OnGameStateReceived(GameStateMessage gameState)
  {
    if (!_isInitialized) return;
    ApplyGameStateUpdates(gameState);
  }

  private void ApplyGameStateUpdates(GameStateMessage gameState)
  {
    foreach (var entityUpdate in gameState.EntityUpdatesPerPlayer.Values)
    {
      UpdateEntity(entityUpdate);
    }
  }

  private void UpdateEntity(EntityStateUpdate update)
  {
    var allEntities = world.Controllers.FlattenControllerHierarchy();
    var entity = allEntities.FirstOrDefault(e => ModuleContainerExtensions.GetEntityId(e) == update.EntityId);

    if (entity != null)
    {
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

    if (networkTransport is LiteNetLibClientTransport liteTransport)
    {
      liteTransport.PollEvents();
    }

    var localPlayer = LocalPlayer;
    if (localPlayer == null) return;

    localPlayer.Update(gameTime);

    if (isHost)
    {
      world.Update(gameTime);
    }

    SendInputToServer(gameTime);
  }

  private void SendInputToServer(GameTime gameTime)
  {
    var inputMessage = new InputMessage
    {
      PlayerId = localPlayerId,
      Tick = (long)(gameTime.TotalGameTime.TotalSeconds * 20),
      IsPressed = input.IsPressed,
      PositionGameCoords = input.PositionGameCoords,
      CameraPosition = LocalPlayer?.Camera?.Position ?? Vector2.Zero,
      CameraZoom = LocalPlayer?.Camera?.Zoom ?? 1f
    };

    networkTransport.SendInputAsync(inputMessage);
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    if (!_isInitialized) return;
    if (LocalPlayer == null) return;

    graphicsDevice.Clear(Color.CornflowerBlue);
    _worldRenderer.Draw(spriteBatch);
  }
}
