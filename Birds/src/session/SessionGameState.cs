using Birds.src.api.client;
using Birds.src.menu;
using Birds.src.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Birds.src.session;

public class SessionGameState : IState
{
  private readonly GameSession _session;
  private readonly GraphicsDevice _graphicsDevice;
  public bool IsLocked { get; set; }

  public SessionGameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input)
  {
    _graphicsDevice = graphicsDevice;
    _session = new SinglePlayerSession(ClientSession.Current, input, game, graphicsDevice);
    _session.InitializeAsync().GetAwaiter().GetResult();
  }

  public void Update(GameTime gameTime) => _session.Update(gameTime);
  public void PostUpdate() { }

  public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    _graphicsDevice.Clear(Color.CornflowerBlue);
    _session.Draw(gameTime, spriteBatch);
  }
}