using Birds.src.containers.composite.blueprints;
using System.Threading.Tasks;

namespace Birds.src.session;

public class MultiPlayerSession : GameSession
{
  private IGameClient gameClient;

  public MultiPlayerSession(string playerId, IGameClient gameClient) : base(playerId)
  {
    this.gameClient = gameClient;

    gameClient.BuildCommitted += OnBuildCommitted;
    gameClient.EntityDestroyed += OnEntityDestroyed;
    gameClient.PlayerCreditsChanged += OnPlayerCreditsChanged;
  }

  public override async Task ConnectAsync()
  {
    await gameClient.ConnectAsync();
  }

  public override async Task DisconnectAsync()
  {
    await gameClient.DisconnectAsync();
  }
}
