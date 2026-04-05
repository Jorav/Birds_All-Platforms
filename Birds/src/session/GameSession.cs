using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Birds.src.player;
using Birds.src.api.client;
using Birds.src.session.world;
using Birds.src.menu;

namespace Birds.src.session;

public abstract class GameSession(ClientSession session, Input input) : IState
{
  protected readonly Input input = input;
  protected readonly string localPlayerId = session.ClientId;
  protected readonly World world = new();
  protected readonly Dictionary<string, Player> players = new();

  public bool IsLocked { get; set; }

  public Player LocalPlayer => players.TryGetValue(localPlayerId, out var p) ? p : null;
  public IReadOnlyDictionary<string, Player> Players => players;

  protected void AddPlayer(Player player) => players[player.Id] = player;
  protected void RemovePlayer(Player player)
  {
    world.RemovePlayer(player.Controller);
    players.Remove(player.Id);
  }

  public abstract Task InitializeAsync();
  public abstract void Update(GameTime gameTime);
  public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
  public abstract Task ConnectAsync();
  public abstract Task DisconnectAsync();
  public virtual void PostUpdate() { }
}
