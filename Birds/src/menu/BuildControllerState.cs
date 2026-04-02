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
using Birds.src.collision.bounding_areas;
using Birds.src.containers.entity;
using Birds.src.containers.composite;
using Birds.src.modules.shared.collision_detection;
using System.Collections.Generic;
using Birds.src.menu.controls;
using System.Linq;
using Birds.src.utility.factories;

namespace Birds.src.menu;

public class BuildControllerState : BuildStateBase
{
  private EntityButtonManager buttonManager;
  private DoubleClickHelper doubleClickHelper;
  private DragHelper dragHelper;
  private string pendingBlueprintName = null;

  private EntityButton _activeDeleteButton;
  private EntityButton _buttonToRemove;
  private string _blueprintToRemove;
  private EntityButton deleteButton;

  public BuildControllerState(
      Game1 game,
      GraphicsDevice graphicsDevice,
      ContentManager content,
      State previousState,
      Input input,
      Controller originalController) : base(game, graphicsDevice, content, previousState, input, originalController)
  {
    editedController = (Controller)originalController.Clone();
    editedController.GetModule<SteeringModule>().actionsLocked = true;
    editedController.Rotation.Value = 0;
    dragHelper = new DragHelper(editedController, Input.Camera);

    doubleClickHelper = new DoubleClickHelper(400);

    InitializeButtons();
    InitializeDeleteButton();
    Input.Camera.Controller = editedController;
    Input.Camera.InBuildScreen = true;
  }

  private void InitializeDeleteButton()
  {
    ISprite deleteIcon = SpriteFactory.GetSprite(ID_SPRITE.DELETE_BUTTON, Vector2.Zero);
    ISprite buttonBg = SpriteFactory.GetSprite(ID_SPRITE.ENTITY_BUTTON_MENU, Vector2.Zero);

    deleteButton = new EntityButton(deleteIcon, buttonBg, autoFit: true)
    {
      Scale = 4f,
      Position = new Vector2(Game1.ScreenWidth - 200, Game1.ScreenHeight - 200),
    };
  }

  private void InitializeButtons()
  {
    buttonManager = new EntityButtonManager(components);
    LoadBlueprintButtons();
  }

  public void LoadBlueprintButtons()
  {
    buttonManager.CreateButtonGrid(
        CompositeControllerFactory.Previews,
        OnBlueprintButtonClicked,
        OnBlueprintButtonLongPressed,
        scale: 3f,
        startX: 20f
    );
  }

  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);

    if (Input.WasJustPressed && _activeDeleteButton != null)
    {
      if (!_activeDeleteButton.IsHovering())
      {
        _activeDeleteButton.IsDeleteMode = false;
        _activeDeleteButton = null;
      }
    }

    if (_buttonToRemove != null)
    {
      CompositeControllerFactory.DeleteBlueprint(_blueprintToRemove);
      _buttonToRemove = null;
      _activeDeleteButton = null;
      LoadBlueprintButtons();
    }
    if (dragHelper.IsDragging)
    {
      deleteButton?.Update(gameTime);
    }
    HandleDeleteEntity();
    dragHelper.Update(gameTime);
    AddEntityIfClicked();
    HandleClickLogic();
  }

  private void HandleDeleteEntity()
  {
    if (dragHelper.IsDragging && Input.WasJustReleased && deleteButton.IsHovering())
    {
      var entityToDelete = dragHelper.DraggedEntity;
      if (entityToDelete != null && editedController.Entities.Count > 1)
      {
        editedController.Entities.Remove(entityToDelete);
        entityToDelete.Dispose();
        dragHelper.Reset();
      }
    }
  }

  private void OnBlueprintButtonClicked(string blueprintName, EntityButton clickedButton)
  {
    if (clickedButton.IsDeleteMode)
    {
      _buttonToRemove = clickedButton;
      _blueprintToRemove = blueprintName;
      return;
    }

    if (_activeDeleteButton != null && _activeDeleteButton != clickedButton)
    {
      _activeDeleteButton.IsDeleteMode = false;
      _activeDeleteButton = null;
    }

    pendingBlueprintName = blueprintName;
  }

  private void OnBlueprintButtonLongPressed(string blueprintName, EntityButton clickedButton)
  {
    if (Enum.TryParse(blueprintName, out ID_ENTITY id))
    {
      if (WorldEntityLoader.Hulls.Contains(id))
        return;
    }

    if (_activeDeleteButton != null && _activeDeleteButton != clickedButton)
    {
      _activeDeleteButton.IsDeleteMode = false;
    }

    clickedButton.IsDeleteMode = !clickedButton.IsDeleteMode;
    _activeDeleteButton = clickedButton.IsDeleteMode ? clickedButton : null;
  }

  private void AddEntityIfClicked()
  {
    if (pendingBlueprintName != null)
    {
      editedController.Entities.AddRange(
          CompositeControllerFactory.CreateComposites(editedController.Position, 1, pendingBlueprintName));

      pendingBlueprintName = null;
    }
  }

  private void HandleClickLogic()
  {
    if (!Input.IsPressed || IsMouseAboveComponent())
      return;

    var bc = editedController.GetModule<BaseCollisionDetectionModule>().BoundingCircle;

    if (bc.Contains(Input.PositionGameCoords))
    {
      foreach (IEntity entity in editedController.Entities)
      {
        if (entity.Contains(Input.PositionGameCoords))
        {
          if (entity is CompositeController && doubleClickHelper.CheckDoubleClick(Input.IsPressed, true))
          {
            game.ChangeState(new EditEntityState(game, graphicsDevice, content, this, backgroundState, input, editedController, entity));
            dragHelper.Reset();
            return;
          }
          break;
        }
      }
    }
    else if (Input.WasJustPressed)
    {
      ReturnToPreviousState();
    }
  }

  protected override void DrawModalContent(GameTime gameTime, SpriteBatch spriteBatch)
  {
    if (dragHelper.IsDragging && deleteButton != null)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, samplerState: SamplerState.AnisotropicClamp);
      deleteButton.Draw(spriteBatch);
      spriteBatch.End();
    }
  }

  protected override void ReturnToPreviousState()
  {
    game.ChangeState(backgroundState);
    originalController.Entities.Set(editedController.Entities);
    originalController.GetModule<SteeringModule>().actionsLocked = false;
    Input.Camera.Controller = originalController;
    Input.Camera.InBuildScreen = false;
    UnlockPlayerActions();
  }
}