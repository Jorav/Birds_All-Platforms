namespace Birds.src.session;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.containers.controller;
using System.Collections.Generic;
using System.Threading.Tasks;
using Birds.src.player;

public abstract class GameSession
{
  protected GameController gameController;
  protected Dictionary<string, Player> players = new();
  protected string localPlayerId;

  public GameController GameController => gameController;
  public Player LocalPlayer => players.TryGetValue(localPlayerId, out var p) ? p : null;
  public IReadOnlyDictionary<string, Player> Players => players;

  public GameSession(string localPlayerId)
  {
    this.localPlayerId = localPlayerId;
    gameController = new GameController();
  }

  protected void AddPlayer(Player player)
  {
    players[player.Id] = player;
  }

  public abstract Task InitializeAsync();
  public abstract void Update(GameTime gameTime);
  public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
  public abstract Task ConnectAsync();
  public abstract Task DisconnectAsync();
}
