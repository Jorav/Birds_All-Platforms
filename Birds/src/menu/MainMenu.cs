using Birds.src.api.client;
using Birds.src.factories;
using Birds.src.menu.controls;
using Birds.src.network;
using Birds.src.player;
using Birds.src.session;
using Birds.src.utility;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
namespace Birds.src.menu;

public class MainMenu : MenuState
{
  public MainMenu(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input)
    : base(game, graphicsDevice, content, input)
  {
    this.input = input;

    Button startSingleplayerButton = new Button(
      SpriteFactory.GetSprite(ID_SPRITE.BUTTON, new Vector2(Game1.ScreenWidth / 2 - 100, Game1.ScreenHeight / 2 - 150)),
      input,
      Game1.font)
    {
      Text = "Singleplayer",
    };
    startSingleplayerButton.Click += StartSingleplayer_Click;

    Button hostGameButton = new Button(
      SpriteFactory.GetSprite(ID_SPRITE.BUTTON, new Vector2(Game1.ScreenWidth / 2 - 100, Game1.ScreenHeight / 2 - 75)),
      input,
      Game1.font)
    {
      Text = "Host Game",
    };
    hostGameButton.Click += HostGame_Click;
    Button editorButton = new Button(
      SpriteFactory.GetSprite(ID_SPRITE.BUTTON, new Vector2(Game1.ScreenWidth / 2 - 100, Game1.ScreenHeight / 2 - 75)),
      input,
      Game1.font)
      {
        Text = "Editor",
      };
    editorButton.Click += Editor_Click;

    Button joinGameButton = new Button(
      SpriteFactory.GetSprite(ID_SPRITE.BUTTON, new Vector2(Game1.ScreenWidth / 2 - 100, Game1.ScreenHeight / 2)),
      input,
      Game1.font)
    {
      Text = "Join Game",
    };
    joinGameButton.Click += JoinGame_Click;

    Button quitButton = new Button(
      SpriteFactory.GetSprite(ID_SPRITE.BUTTON, new Vector2(Game1.ScreenWidth / 2 - 100, Game1.ScreenHeight / 2 + 75)),
      input,
      Game1.font)
    {
      Text = "Quit",
    };
    quitButton.Click += Quit_Click;

    Sprite background = SpriteFactory.GetSprite(
        ID_SPRITE.BACKGROUND_GRAY,
        new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2),
        SpriteFactory.GetBackgroundScale(ID_SPRITE.BACKGROUND_GRAY)
    );

    ButtonContainer container = new ButtonContainer(
      ID_POSITION.POSITION_MIDDLE,
      new List<Button> { startSingleplayerButton, hostGameButton, editorButton, joinGameButton, quitButton }
    );

    components = new List<IComponent>()
    {
      background,
      container,
    };
  }

  private void StartSingleplayer_Click(object sender, EventArgs e)
  {
    var singlePlayerSession = new SinglePlayerSession(ClientSession.Current, input, game, graphicsDevice);
    singlePlayerSession.Initialize().GetAwaiter().GetResult();
    game.ChangeState(singlePlayerSession);
  }

  private void Editor_Click(object sender, EventArgs e)
  {
    //game.ChangeState(new WorldEditor(game, graphicsDevice, content, input));
  }

  private async void HostGame_Click(object sender, EventArgs e)
  {
    try
    {
      bool serverStarted = await game.ServerManager.StartLocalServer();
      if (!serverStarted) return;

      var networkTransport = new LiteNetLibClientTransport(
          game.ServerManager.GetLocalServerAddress(),
          game.ServerManager.GetLocalServerPort()
      );

      var multiplayerSession = new MultiplayerSession(
          ClientSession.Current,
          input,
          game,
          graphicsDevice,
          networkTransport,
          isHost: true
      );

      await multiplayerSession.Initialize();
      game.ChangeState(multiplayerSession);
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Failed to host game: {ex.Message}");
    }
  }

  private void JoinGame_Click(object sender, EventArgs e)
  {
    // TODO: Show input dialog for server address, then create MultiplayerSession
    throw new NotImplementedException("Join game not yet implemented");
  }

  private void Quit_Click(object sender, EventArgs e)
  {
    game.Exit();
  }
}
