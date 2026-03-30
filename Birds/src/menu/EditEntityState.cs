using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.menu.controls;
using Birds.src.modules.composite;
using Birds.src.modules.entity;
using Birds.src.modules.shared.collision_detection;
using Birds.src.storage.implementations;
using Birds.src.utility;
using Birds.src.utility.factories;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.menu;

public class EditEntityState : BuildStateBase
{
  private IEntity editedEntity;
  private IEntity originalEntity;
  private ID_ENTITY idToBeAddded;
  private EntityButton clicked;
  private EntityButton previouslyClicked;
  private EntityButtonManager buttonManager;

  private bool isSaveModalOpen = false;
  private TextInputBox textInput;
  private Button openSaveModalButton;
  private Button confirmSaveButton;
  private Button cancelSaveButton;
  private SpriteFont font;
  private bool saveAndExit;

  public EditEntityState(
      Game1 game,
      GraphicsDevice graphicsDevice,
      ContentManager content,
      State stateToReturnTo,
      State stateToDraw,
      Input input,
      Controller originalController,
      IEntity editedEntity) : base(game, graphicsDevice, content, stateToDraw, input, originalController)
  {
    this.backgroundState = stateToReturnTo;

    this.editedEntity = (IEntity)editedEntity.Clone();
    originalEntity = editedEntity;
    editedController = ControllerFactory.Create(
        new List<IEntity> { this.editedEntity },
        ID_CONTROLLER.DEFAULT
    );

    idToBeAddded = ID_ENTITY.DEFAULT;
    font = Game1.font;

    InitializeButtons();
    InitializeModalComponents();
    AddOpenLinks();
    Input.Camera.Controller = editedController;
    Input.Camera.InBuildScreen = true;
  }

  private void InitializeButtons()
  {
    buttonManager = new EntityButtonManager(components);
    LoadPartButtons();

    openSaveModalButton = new Button(SpriteFactory.GetSprite(ID_SPRITE.BUTTON, Vector2.Zero, 2f), font)
    {
      Text = "Save",
      Position = new Vector2(Game1.ScreenWidth - 200, Game1.ScreenHeight - 80),
    };
    openSaveModalButton.Click += OpenSaveModalButton_Click;
    components.Add(openSaveModalButton);
  }

  private void LoadPartButtons()
  {
    buttonManager.CreateButtonGrid(
        WorldEntityFactory.Previews,
        OnPartButtonClicked,
        (id, btn) => { },
        scale: 3f,
        buttonsPerRow: 3,
        startX: 50f,
        filter: partID => WorldEntityLoader.HasModule(partID, ID_MODULE.LinkModule) && partID != ID_ENTITY.FILLER
    );

    clicked = buttonManager.GetFirstButton();
    if (clicked != null)
    {
      buttonManager.SetFirstButtonSelected();
      idToBeAddded = WorldEntityFactory.Previews.First(kvp =>
          WorldEntityLoader.HasModule(kvp.Key, ID_MODULE.LinkModule) &&
          kvp.Key != ID_ENTITY.FILLER).Key;
    }
  }

  private void InitializeModalComponents()
  {
    textInput = new TextInputBox(new Rectangle(Game1.ScreenWidth / 2 - 150, Game1.ScreenHeight / 2 - 20, 300, 40), font, graphicsDevice);

    confirmSaveButton = new Button(SpriteFactory.GetSprite(ID_SPRITE.BUTTON, Vector2.Zero, 2f), font)
    {
      Text = "Confirm",
      Position = new Vector2(Game1.ScreenWidth / 2 - 150, Game1.ScreenHeight / 2 + 30),
    };
    confirmSaveButton.Click += ConfirmSaveButton_Click;

    cancelSaveButton = new Button(SpriteFactory.GetSprite(ID_SPRITE.BUTTON, Vector2.Zero, 2f), font)
    {
      Text = "Cancel",
      Position = new Vector2(Game1.ScreenWidth / 2 + 10, Game1.ScreenHeight / 2 + 30),
    };
    cancelSaveButton.Click += CancelSaveButton_Click;
  }

  private void OnPartButtonClicked(ID_ENTITY partID, EntityButton clickedButton)
  {
    idToBeAddded = partID;
    clicked = clickedButton;
  }

  private void OpenSaveModalButton_Click(object sender, EventArgs e)
  {
    isSaveModalOpen = true;
    textInput.IsActive = true;
  }

  private void CancelSaveButton_Click(object sender, EventArgs e)
  {
    isSaveModalOpen = false;
    textInput.IsActive = false;
  }

  private async void ConfirmSaveButton_Click(object sender, EventArgs e)
  {
    if (string.IsNullOrWhiteSpace(textInput.Text))
    {
      return;
    }
    saveAndExit = true;
  }

  private async void SaveEntityAnUpdatePreviousState()
  {
    var managementModule = editedEntity.GetModule<LinkManagementModule>();
    managementModule.ClearFillerEntities();
    var entitiesToSave = editedEntity.Entities.Cast<WorldEntity>().ToList();
    var blueprint = BlueprintFactory.CreateBlueprint(entitiesToSave, textInput.Text);
    await new JsonBlueprintStorage().SaveBlueprintAsync(blueprint);
    CompositeControllerFactory.InitializePreviews();
    isSaveModalOpen = false;
    textInput.IsActive = false;

    if (backgroundState is BuildControllerState buildState)
    {
      buildState.LoadBlueprintButtons();
    }
  }

  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
    if (saveAndExit)
    {
      SaveEntityAnUpdatePreviousState();
      ReturnToPreviousState();
      return;
    }

    if (isSaveModalOpen)
    {
      textInput.Update(gameTime);
      confirmSaveButton.Update(gameTime);
      cancelSaveButton.Update(gameTime);
      return;
    }

    UpdateClickedState();

    if (Input.IsPressed && !IsMouseAboveComponent())
    {
      var bc = editedEntity.GetModule<BaseCollisionDetectionModule>().BoundingCircle;
      if (bc.Contains(Input.PositionGameCoords))
      {
        AddEntityIfFillerClicked();
      }
      else if (Input.WasPressed)
      {
        ReturnToPreviousState();
      }
    }
  }

  private void UpdateClickedState()
  {
    if (clicked != previouslyClicked)
    {
      if (previouslyClicked != null)
        previouslyClicked.IsClicked = false;
      previouslyClicked = clicked;
      if (clicked != null)
        clicked.IsClicked = true;
    }
  }

  private void AddEntityIfFillerClicked()
  {
    var linkManagementModule = editedEntity.GetModule<LinkManagementModule>();
    foreach (IEntity entity in linkManagementModule.fillerEntities)
    {
      if (entity.Contains(Input.PositionGameCoords))
      {
        bool successfullyReplaced = editedEntity.ReplaceAndAttach(entity, WorldEntityFactory.CreateEntities(Vector2.Zero, 1, idToBeAddded, isComposite: true).First());
        if (successfullyReplaced)
        {
          linkManagementModule.AddFillerEntities();
        }
        break;
      }
    }
  }

  private void AddOpenLinks()
  {
    var managementModule = editedEntity.GetModule<LinkManagementModule>();
    managementModule.AddFillerEntities();
  }

  protected override void ReturnToPreviousState()
  {
    editedController.Entities.Set(Enumerable.Empty<IEntity>());
    editedController.Dispose();
    game.ChangeState(backgroundState);
    var managementModule = editedEntity.GetModule<LinkManagementModule>();
    managementModule.ClearFillerEntities();
    originalController.Entities.Remove(originalEntity);
    originalEntity.Dispose();
    originalController.Entities.Add(editedEntity);
    Input.Camera.Controller = originalController;
    Input.Camera.Position = originalController.Position;
    Input.Camera.InBuildScreen = true;
  }

  protected override void DrawCustomContent(SpriteBatch spriteBatch)
  {
    DrawAvailableLinks(spriteBatch);
  }

  protected override void DrawModalContent(GameTime gameTime, SpriteBatch spriteBatch)
  {
    if (isSaveModalOpen)
    {
      spriteBatch.Begin();
      var darkOverlay = new Texture2D(graphicsDevice, 1, 1);
      darkOverlay.SetData(new[] { new Color(0, 0, 0, 150) });
      spriteBatch.Draw(darkOverlay, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);
      spriteBatch.DrawString(font, "Name the blueprint:", new Vector2(Game1.ScreenWidth / 2 - 150, Game1.ScreenHeight / 2 - 50), Color.White);
      textInput.Draw(spriteBatch);
      confirmSaveButton.Draw(spriteBatch);
      cancelSaveButton.Draw(spriteBatch);
      spriteBatch.End();
    }
  }

  private void DrawAvailableLinks(SpriteBatch spriteBatch)
  {
    var pixelTexture = new Texture2D(graphicsDevice, 1, 1);
    pixelTexture.SetData(new[] { Color.Green });

    var fillerEntity = WorldEntityFactory.GetEntity(Vector2.Zero, ID_ENTITY.FILLER, false);
    var fillerLinkModule = fillerEntity.GetModule<LinkModule>();
    var fillerLink = fillerLinkModule?.Links.Count > 0 ? fillerLinkModule.Links[0] : null;

    foreach (IEntity entity in editedEntity.Entities)
    {
      var linkModule = entity.GetModule<LinkModule>();
      foreach (var link in linkModule.Links)
      {
        if (link.ConnectionAvailable)
        {
          spriteBatch.Draw(pixelTexture, new Rectangle((int)link.AbsolutePositionOnEntity.X - 2, (int)link.AbsolutePositionOnEntity.Y - 2, 4, 4), Color.Yellow);

          if (fillerLink != null)
          {
            Vector2 connectionPos = link.GetConnectionPosition(fillerLink);
            spriteBatch.Draw(pixelTexture, new Rectangle((int)connectionPos.X - 2, (int)connectionPos.Y - 2, 4, 4), Color.Blue);
          }
        }
      }
    }

    fillerEntity.Dispose();
  }
}