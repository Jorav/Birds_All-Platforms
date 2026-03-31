using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.utility;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Birds.src.factories;
using Birds.src.events;
using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.modules.shared.collision_detection;

namespace Birds.src.menu;

public class GameState : State
{
  protected GameController controller;
  protected List<Background> backgrounds;
  protected List<Background> foregrounds;
  protected State previousState;
  public List<IEntity> newEntities;
  public static Controller Player { get; set; }
  public Camera Camera { get; set; }

  private DoubleClickHelper doubleClickHelper;

  public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input, [OptionalAttribute] State previousState) : base(game, graphicsDevice, content, input)
  {
    controller = new GameController();
    backgrounds = new List<Background>();
    foregrounds = new List<Background>();
    this.previousState = previousState;
    newEntities = new List<IEntity>();

    doubleClickHelper = new DoubleClickHelper(400);

    if (Player == null)
    {
      Player = ControllerFactory.Create(
        CompositeControllerFactory.CreateComposites(Vector2.Zero, 1, CompositeControllerFactory.DEFAULT_SINGLE),
        ID_CONTROLLER.PLAYER
        );
      controller.Add(Player);
    }
    Camera = new Camera(Player);
    Input.Camera = Camera;
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    spriteBatch.Begin(transformMatrix: Camera.Transform, sortMode: SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.AnisotropicClamp);
    graphicsDevice.Clear(Color.CornflowerBlue);
    foreach (Background b in backgrounds)
    {
      b.Draw(spriteBatch);
    }
    controller.Draw(spriteBatch);
    foreach (Background f in foregrounds)
    {
      f.Draw(spriteBatch);
    }
    spriteBatch.End();
  }

  public override void PostUpdate()
  {
  }

  public override void Update(GameTime gameTime)
  {
    RunGame(gameTime);
    HandleScroll();
    CheckKeyboardShortcuts();
    CheckDoubleClick();
  }

  private void HandleScroll()
  {
    Input.HandleZoom();
  }

  private void CheckKeyboardShortcuts()
  {
    if (input.BuildClicked)
    {
      game.ChangeState(new BuildControllerState(game, graphicsDevice, content, this, input, Player));
      return;
    }
  }

  private void CheckDoubleClick()
  {
    bool playerClicked = Player.GetModule<BaseCollisionDetectionModule>().BoundingCircle.Contains(Input.PositionGameCoords);

    if (doubleClickHelper.CheckDoubleClick(Input.IsPressed, playerClicked))
    {
      HandleDoubleClick();
    }
  }

  private void HandleDoubleClick()
  {
    game.ChangeState(new BuildControllerState(game, graphicsDevice, content, this, input, Player));
  }

  public void RunGame(GameTime gameTime)
  {
    controller.Update(gameTime);

    foreach (Background b in backgrounds)
      b.Update(gameTime);
    foreach (Background f in foregrounds)
      f.Update(gameTime);
  }
}