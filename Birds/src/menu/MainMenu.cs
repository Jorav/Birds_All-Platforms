using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.menu.controls;
using System;
using System.Collections.Generic;
using Birds.src.utility;
using Birds.src.visual;
using Birds.src.factories;
using Birds.src.player;
using Birds.src.session;

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
      new List<Button> { startSingleplayerButton, editorButton, hostGameButton, joinGameButton, quitButton }
    );

    components = new List<IComponent>()
    {
      background,
      container,
    };
  }

  private void StartSingleplayer_Click(object sender, EventArgs e)
  {
    var sessionState = new SessionGameState(game, graphicsDevice, content, input);
    game.ChangeState(sessionState);
  }

  private void Editor_Click(object sender, EventArgs e)
  {
    //game.ChangeState(new WorldEditor(game, graphicsDevice, content, input));
  }

  private void HostGame_Click(object sender, EventArgs e)
  {
    // TODO: Start Birds.Server process, then create MultiplayerSession
    throw new NotImplementedException("Host game not yet implemented");
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
