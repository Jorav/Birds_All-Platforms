namespace Birds.src.session;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.containers.controller;
using Birds.src.utility;
using Birds.src.factories;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SinglePlayerSession : GameSession
{
  private LocalGameServer localServer;

  public SinglePlayerSession(string playerId = "local") : base(playerId)
  {
    localServer = new LocalGameServer(gameController);
  }

  public override async Task InitializeAsync()
  {
    var playerController = ControllerFactory.Create(
        CompositeControllerFactory.CreateComposites(Vector2.Zero, 1, CompositeControllerFactory.DEFAULT_SINGLE),
        ID_CONTROLLER.PLAYER
    );

    var player = new Player(
        id: localPlayerId,
        controller: playerController
    );

    AddPlayer(player);
    gameController.Add(playerController);

    await Task.CompletedTask;
  }

  public override void Update(GameTime gameTime)
  {
    gameController.Update(gameTime);
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    var localPlayer = LocalPlayer;
    if (localPlayer == null) return;

    spriteBatch.Begin(
        transformMatrix: localPlayer.Camera.Transform,
        sortMode: SpriteSortMode.Deferred,
        blendState: BlendState.NonPremultiplied,
        samplerState: SamplerState.AnisotropicClamp
    );

    gameController.Draw(spriteBatch);

    spriteBatch.End();
  }

  public override Task ConnectAsync()
  {
    return Task.CompletedTask;
  }

  public override Task DisconnectAsync()
  {
    return Task.CompletedTask;
  }
}
