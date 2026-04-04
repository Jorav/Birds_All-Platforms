using Birds.src.api.client;
using Birds.src.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;

namespace Birds.src.session;

public class MultiPlayerSession(ClientSession session, Input input, IGameClient gameClient)
    : GameSession(session, input)
{
  public override async Task ConnectAsync() => await gameClient.ConnectAsync();
  public override async Task DisconnectAsync() => await gameClient.DisconnectAsync();
  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) => throw new NotImplementedException();
  public override Task InitializeAsync() => throw new NotImplementedException();
  public override void Update(GameTime gameTime) => throw new NotImplementedException();
}
