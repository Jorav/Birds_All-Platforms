using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Birds.src.utility;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Birds.src.factories;
using Birds.src.modules.shared.bounding_area;
using Birds.src.events;
using Birds.src.containers.controller;
using Birds.src.containers.entity;

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

  private bool wasPressed = true;
  Stopwatch timer = new Stopwatch();
  private int doubleClickTreshold = 400;

  public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input, [OptionalAttribute] State previousState) : base(game, graphicsDevice, content, input)
  {
    controller = new GameController();
    backgrounds = new List<Background>();
    foregrounds = new List<Background>();
    this.previousState = previousState;
    newEntities = new List<IEntity>();
    if (Player == null)
    {
      Player = ControllerFactory.Create(Vector2.Zero, ID_CONTROLLER.PLAYER);
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
    //throw new NotImplementedException();
  }

  public override void Update(GameTime gameTime)
  {
    /*if (Player.Input.PauseClicked)
        game.ChangeState(new PauseState(game, graphicsDevice, content, this, input));*/
    /*else if (Player.Input.BuildClicked)
        if (Player.Entities != null && Player.Entities.Count>0)
            game.ChangeState(new BuildOverviewState(game, graphicsDevice, content, this, input, Player));*/
    /*if (input.EnterClicked && previousState != null)
    {
        game.ChangeState(previousState);
        if (previousState is IPlayable p)
            input.Camera = p.Camera;
    }*/
    RunGame(gameTime);
    HandleScroll();
    CheckClickOnPlayer();
    CheckDoubleClick();
    wasPressed = Input.IsPressed;
  }

  private void HandleScroll()
  {
    Input.HandleZoom();
  }

  private void CheckClickOnPlayer()
  {
    if (!wasPressed && Input.IsPressed && Player.GetModule<BCCollisionDetectionModule>().BoundingCircle.Contains(Input.PositionGameCoords))
    {
    }
  }

  private void CheckDoubleClick()
  {
    if (!wasPressed && Input.IsPressed)
    {
      if (timer.IsRunning)
      {
        if (timer.ElapsedMilliseconds < doubleClickTreshold && Player.GetModule<BCCollisionDetectionModule>().BoundingCircle.Contains(Input.PositionGameCoords))
          HandleDoubleClick();
        timer.Reset();
      }
      else
      {
        if (Player.GetModule<BCCollisionDetectionModule>().BoundingCircle.Contains(Input.PositionGameCoords))
        {
          timer.Start();
        }
      }
    }
    if (timer.IsRunning && timer.ElapsedMilliseconds >= doubleClickTreshold)
      timer.Reset();
  }

  private void HandleDoubleClick()
  {
    game.ChangeState(new BuildControllerState(game, graphicsDevice, content, this, input, Player));
  }

  public void RunGame(GameTime gameTime)
  {

    //UPDATE
    controller.Update(gameTime);

    //ADD NEW ENTITIES
    /*foreach (IEntity c in controllers)
        if (c is Controller cc)
        {
            foreach (IEntity cSeperated in cc.ExtractAllSeperatedEntities())
                newEntities.Add(cSeperated);
            cc.SeperatedEntities.Clear();
        }
    foreach (IEntity c in newEntities)
        controllers.Add(c);
    newEntities.Clear();*/

    //BACKGROUNDS
    foreach (Background b in backgrounds)
      b.Update(gameTime);
    foreach (Background f in foregrounds)
      f.Update(gameTime);

  }
}

