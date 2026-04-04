using Birds.src.containers.controller;
using Microsoft.Xna.Framework;

namespace Birds.src.player;

public class Player
{
  public string Id { get; }
  public Input Input { get; }
  public Controller Controller { get; private set; }
  public Camera Camera { get; }
  public PlayerState State { get; set; } = PlayerState.Playing;

  public Player(string id, Input input)
  {
    Id = id;
    Input = input;
    Camera = new Camera();
    input.Camera = Camera;
  }

  public void SetController(Controller controller)
  {
    Controller = controller;
    Camera.TrackedController = controller;
  }

  public void Update(GameTime gameTime)
  {
    Input.HandleZoom();
    Camera.Update(gameTime);
  }
}
