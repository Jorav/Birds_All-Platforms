using Birds.src.api.client;
using Birds.src.modules.shared.collision_detection;
using Birds.src.player;
using Birds.src.session.world;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace Birds.src.session;

public class SinglePlayerSession(ClientSession session, Input input, Game1 game, GraphicsDevice graphicsDevice)
    : GameSession(session, input)
{
  private readonly DoubleClickHelper doubleClickHelper = new(input, 400);
  private WorldRenderer worldRenderer;

  public override async Task InitializeAsync()
  {
    WorldInitializer.Initialize(world);

    var player = new Player(localPlayerId, input);
    var playerController = world.AddPlayer(input);
    player.SetController(playerController);
    AddPlayer(player);
    worldRenderer = new WorldRenderer(world, player.Camera);

    await Task.CompletedTask;
  }

  public override void Update(GameTime gameTime)
  {
    var localPlayer = LocalPlayer;
    if (localPlayer == null) return;

    localPlayer.Update(gameTime);
    world.Update(gameTime);

    CheckKeyboardShortcuts(localPlayer);
    CheckDoubleClick(localPlayer);
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    var localPlayer = LocalPlayer;
    if (localPlayer == null) return;

    graphicsDevice.Clear(Color.CornflowerBlue);
    worldRenderer.Draw(spriteBatch);
  }

  private void CheckKeyboardShortcuts(Player localPlayer)
  {
    if (input.BuildClicked)
      OpenBuildState(localPlayer);
  }

  private void CheckDoubleClick(Player localPlayer)
  {
    bool playerClicked = localPlayer.Controller
        .GetModule<BaseCollisionDetectionModule>()
        .BoundingCircle.Contains(input.PositionGameCoords);

    if (doubleClickHelper.CheckDoubleClick(input.IsPressed, playerClicked))
      OpenBuildState(localPlayer);
  }

  private void OpenBuildState(Player localPlayer)
  {
    // TODO: revisit when redoing build state
  }

  public override Task ConnectAsync() => Task.CompletedTask;
  public override Task DisconnectAsync() => Task.CompletedTask;
}
