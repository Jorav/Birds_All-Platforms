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

namespace Birds.src.menu;

public class BuildControllerState : BuildStateBase
{
  private BoundingCircle selectionCircle;
  private EntityButtonManager buttonManager;
  private DoubleClickHelper doubleClickHelper;
  private const float selectionBuffer = 1.5f;

  private string pendingBlueprintName = null;

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

    doubleClickHelper = new DoubleClickHelper(400);

    InitializeSelectionCircle();
    InitializeButtons();
    Input.Camera.Controller = editedController;
    Input.Camera.InBuildScreen = true;
  }

  private void InitializeSelectionCircle()
  {
    var boundingCircle = editedController.GetModule<BaseCollisionDetectionModule>().BoundingCircle;
    selectionCircle = BoundingAreaFactory.GetCircle(boundingCircle.Position, boundingCircle.Radius * selectionBuffer);
  }

  private void InitializeButtons()
  {
    buttonManager = new EntityButtonManager(components);
    LoadBlueprintButtons();
  }

  public void LoadBlueprintButtons()
  {
    buttonManager.CreateButtonGrid(
        CompositeControllerFactory.Previews.Take(10),
        OnBlueprintButtonClicked,
        OnBlueprintButtonLongPressed,
        scale: 3f,
        startX: 20f
    );
  }

  private void OnBlueprintButtonClicked(string blueprintName, EntityButton clickedButton)
  {
    pendingBlueprintName = blueprintName;
  }

  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
    var collisionDetector = editedController.GetModule<GroupCollisionDetectionModule>();
    collisionDetector.AddInternalCollisions();

    UpdateSelectionCircle();
    AddEntityIfClicked();
    HandleClickLogic();
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

  private void UpdateSelectionCircle()
  {
    var boundingCircle = editedController.GetModule<BaseCollisionDetectionModule>().BoundingCircle;
    selectionCircle.Radius = boundingCircle.Radius * selectionBuffer;
    selectionCircle.Position = boundingCircle.Position;
  }

  private void HandleClickLogic()
  {
    if (!Input.IsPressed || IsMouseAboveComponent())
      return;

    if (Input.WasPressed)
    {
      var playerWithBufferClicked = selectionCircle.Contains(Input.PositionGameCoords);

      if (!playerWithBufferClicked)
      {
        ReturnToPreviousState();
        return;
      }
    }

    foreach (IEntity entity in editedController.Entities)
    {
      if (entity.Contains(Input.PositionGameCoords) && entity is CompositeController)
      {
        if (doubleClickHelper.CheckDoubleClick(Input.IsPressed, true))
        {
          game.ChangeState(new EditEntityState(game, graphicsDevice, content, this, backgroundState, input, editedController, entity));
          return;
        }
        return;
      }
    }

    if (doubleClickHelper.CheckDoubleClick(Input.IsPressed, true))
    {
      pendingBlueprintName = CompositeControllerFactory.DEFAULT_SINGLE;
    }
  }

  private void OnBlueprintButtonLongPressed(string arg1, EntityButton button)
  {
    Debug.WriteLine("Button held! Show delete option.");
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