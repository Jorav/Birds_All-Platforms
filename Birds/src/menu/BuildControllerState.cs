using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Birds.src.utility;
using Birds.src.factories;
using System.Diagnostics;
using Birds.src.modules.controller.steering;
using Birds.src.events;
using Birds.src.containers.controller;
using Birds.src.visual;
using Birds.src.modules.collision;
using Birds.src.collision.bounding_areas;
using Birds.src.containers.entity;
using Birds.src.containers.composite;
using Birds.src.modules.shared.collision_detection;

namespace Birds.src.menu;

public class BuildControllerState : MenuState
{
  public State previousState;
  //public MenuController menuController;
  //protected Controller controllerEdited;
  protected Controller controllerEdited;
  protected Controller originalController;
  protected bool buildMode;
  private readonly Sprite overlay;
  protected Color originalColor;
  private bool wasPressed = true;
  private bool playerLastClicked = false;
  Stopwatch timer = new Stopwatch();
  private int doubleClickTreshold = 400;
  private const float selectionBuffer = 1.5f;
  private BoundingCircle selectionCircle;

  public BuildControllerState(
    Game1 game, 
    GraphicsDevice graphicsDevice, 
    ContentManager content, 
    State previousState, 
    Input input, 
    Controller originalController) : base(game, graphicsDevice, content, input)
  {
    controllerEdited = (Controller)originalController.Clone();
    this.originalController = originalController;

    Input.Camera.Controller = controllerEdited;
    controllerEdited.GetModule<SteeringModule>().actionsLocked = true;
    Input.Camera.InBuildScreen = true;
    this.previousState = previousState;
    if (previousState is GameState)
      GameState.Player.GetModule<SteeringModule>().actionsLocked = true;
    overlay = SpriteFactory.GetSprite(ID_SPRITE.BACKGROUND_WHITE, new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2), SpriteFactory.textures[(int)ID_SPRITE.BACKGROUND_WHITE].Height / Game1.ScreenHeight);
    components = new();
    var boundingCircle = controllerEdited.GetModule<BaseCollisionDetectionModule>().BoundingCircle;
    selectionCircle = BoundingAreaFactory.GetCircle(boundingCircle.Position, boundingCircle.Radius * selectionBuffer);
  }

  /*protected List<IEntity> CopyEntitiesFromController(Controller controller)
  {
      List<IEntity> collidables = new List<IEntity>();
      foreach (IEntity c in controller.Controllables)
          collidables.Add((IEntity)c.Clone());
      return collidables;
  }*/

  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
    Input.HandleZoom();
    controllerEdited.Update(gameTime);
    var collisionDetector = controllerEdited.GetModule<GroupCollisionDetectionModule>();
    collisionDetector.CollisionManager.AddInternalCollisionsToEntities();

    var boundingCircle = controllerEdited.GetModule<BaseCollisionDetectionModule>().BoundingCircle;
    selectionCircle.Radius = boundingCircle.Radius * selectionBuffer;
    selectionCircle.Position = boundingCircle.Position;
    HandleClick();
    wasPressed = Input.IsPressed;
  }

  private void HandleClick()
  {
    if (timer.IsRunning && timer.ElapsedMilliseconds >= doubleClickTreshold)
    {
      timer.Stop();
      timer.Reset();
    }
    if (wasPressed || !Input.IsPressed)
    {
      return;
    }
    var playerWithBufferClicked = selectionCircle.Contains(Input.PositionGameCoords);
    if (!playerWithBufferClicked)
    {
      ReturnToPreviousState();
      return;
    }
    if (CheckIfEntityClicked())
    {
      return;
    }
    if (!timer.IsRunning)
    {
      timer.Reset();
      timer.Start();
      playerLastClicked = true;
      return;
    }
    if (playerLastClicked)
    {
      controllerEdited.Entities.AddRange(CompositeControllerFactory.CreateComposites(Input.PositionGameCoords, 1, ID_COMPOSITE.DEFAULT_SINGLE));
      timer.Stop();
      timer.Reset();
    }
  }

  private bool CheckIfEntityClicked()
  {
    foreach (IEntity entity in controllerEdited.Entities)
    {
      if (entity.Contains(Input.PositionGameCoords) && entity is CompositeController)
      {
        game.ChangeState(new EditEntityState(game, graphicsDevice, content, this, previousState, input, controllerEdited, entity));
        return true;
      }
    }
    return false;
  }

  private void ReturnToPreviousState()
  {
    game.ChangeState(previousState);
    originalController.Entities.Set(controllerEdited.Entities);
    originalController.GetModule<SteeringModule>().actionsLocked = false;
    Input.Camera.Controller = originalController;
    Input.Camera.InBuildScreen = false;
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    previousState.Draw(gameTime, spriteBatch);
    spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.AnisotropicClamp);
    overlay.Draw(spriteBatch);
    spriteBatch.End();
    spriteBatch.Begin(transformMatrix: Input.Camera.Transform, sortMode: SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, samplerState: SamplerState.AnisotropicClamp);
    controllerEdited.Draw(spriteBatch);
    spriteBatch.End();
    base.Draw(gameTime, spriteBatch);
  }
}

