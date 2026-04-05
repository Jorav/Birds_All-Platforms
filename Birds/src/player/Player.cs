using Birds.src.containers.controller;
using Microsoft.Xna.Framework;

namespace Birds.src.player;

public class Player
{
  public string Id { get; }
  public IInputState InputState { get; }
  public Controller Controller { get; private set; }
  public Camera Camera { get; }
  public PlayerState State { get; set; } = PlayerState.Playing;

  // Client
  public Player(string id, Input input)
  {
    Id = id;
    InputState = input;
    Camera = new Camera();
    input.Camera = Camera;
  }

  // Server
  public Player(string id, NetworkInputState networkInput)
  {
    Id = id;
    InputState = networkInput;
    Camera = null;
  }

  public void SetController(Controller controller)
  {
    Controller = controller;
    if (Camera != null)
      Camera.TrackedController = controller;
  }

  public void Update(GameTime gameTime)
  {
    if (InputState is Input input)
      input.HandleZoom();

    Camera?.Update(gameTime);
  }
}
