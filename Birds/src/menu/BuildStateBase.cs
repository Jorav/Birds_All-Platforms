using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Birds.src.utility;
using Birds.src;
using Birds.src.containers.controller;
using Birds.src.menu;
using Birds.src.factories;
using Birds.src.visual;
using System.Linq;
using Birds.src.menu.controls;
using Birds.src.modules.controller.steering;
using Birds.src.events;
using Birds.src.player;

namespace Birds.src.menu;

public abstract class BuildStateBase : MenuState
{
  protected Controller editedController;
  protected Controller originalController;
  protected State backgroundState;
  protected readonly Sprite overlay;

  protected BuildStateBase(
      Game1 game,
      GraphicsDevice graphicsDevice,
      ContentManager content,
      State backgroundState,
      Input input,
      Controller originalController) : base(game, graphicsDevice, content, input)
  {
    this.backgroundState = backgroundState;
    this.originalController = originalController;
    overlay = SpriteFactory.GetSprite(
      ID_SPRITE.BACKGROUND_WHITE,
      new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2),
      SpriteFactory.GetBackgroundScale(ID_SPRITE.BACKGROUND_WHITE)
    );

    InitializeCamera();
    LockPlayerActions();
  }

  protected virtual void InitializeCamera()
  {
    input.Camera.TrackedController = editedController;
    input.Camera.InBuildScreen = true;
  }

  protected virtual void LockPlayerActions()
  {
    if (backgroundState is GameState)
      GameState.Player.GetModule<SteeringModule>().actionsLocked = true;
  }

  protected virtual void UnlockPlayerActions()
  {
    if (backgroundState is GameState)
      GameState.Player.GetModule<SteeringModule>().actionsLocked = false;
  }

  protected virtual bool IsMouseAboveComponent()
  {
    return components.Any(c => c is Button b && b.IsHovering());
  }

  protected virtual void HandleCommonInput()
  {
    input.HandleZoom();

    if (input.BuildClicked)
    {
      ReturnToPreviousState();
    }
  }

  protected abstract void ReturnToPreviousState();

  public override void Update(GameTime gameTime)
  {
    HandleCommonInput();
    editedController?.Update(gameTime);
    input.Camera.UpdateTransformMatrix();//TODO: Remove this since its a quick fix
    base.Update(gameTime);
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    backgroundState.Draw(gameTime, spriteBatch);

    spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.AnisotropicClamp);
    overlay.Draw(spriteBatch);
    spriteBatch.End();

    spriteBatch.Begin(transformMatrix: input.Camera.Transform, sortMode: SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, samplerState: SamplerState.AnisotropicClamp);
    editedController?.Draw(spriteBatch);
    DrawCustomContent(spriteBatch);
    spriteBatch.End();

    base.Draw(gameTime, spriteBatch);
    DrawModalContent(gameTime, spriteBatch);
  }

  protected virtual void DrawCustomContent(SpriteBatch spriteBatch) { }
  protected virtual void DrawModalContent(GameTime gameTime, SpriteBatch spriteBatch) { }
}