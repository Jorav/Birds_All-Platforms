using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.menu.controls;
using Birds.src.modules.composite;
using Birds.src.modules.entity;
using Birds.src.modules.shared.collision_detection;
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

public class EditEntityState : MenuState
{
  State previousState;
  State backgroundState;
  IEntity editedEntity;
  IEntity originalEntity;
  Controller editedController;
  Controller originalController;
  ID_ENTITY idToBeAddded;
  EntityButton clicked;
  EntityButton previouslyClicked;
  private bool wasPressed = true;
  private readonly Sprite overlay;
  private bool isSaveModalOpen = false;
  private TextInputBox textInput;
  private Button openSaveModalButton;
  private Button confirmSaveButton;
  private Button cancelSaveButton;
  private SpriteFont font;
  private bool saveAndExit;
  private List<EntityButton> partButtons = new List<EntityButton>();

  public EditEntityState(
    Game1 game,
    GraphicsDevice graphicsDevice,
    ContentManager content,
    State stateToReturnTo,
    State stateToDraw,
    Input input,
    Controller originalController,
    IEntity editedEntity) : base(game, graphicsDevice, content, input)
  {
    this.originalController = originalController;
    this.previousState = stateToReturnTo;
    this.backgroundState = stateToDraw;
    components = new List<IComponent>();
    this.editedEntity = (IEntity)editedEntity.Clone();
    originalEntity = editedEntity;
    editedController = ControllerFactory.Create(
         new List<IEntity> { this.editedEntity },
        ID_CONTROLLER.DEFAULT
        );
    Input.Camera.Controller = editedController;
    Input.Camera.InBuildScreen = true;
    idToBeAddded = ID_ENTITY.DEFAULT;

    overlay = SpriteFactory.GetSprite(ID_SPRITE.BACKGROUND_WHITE, new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2), SpriteFactory.textures[(int)ID_SPRITE.BACKGROUND_WHITE].Height / Game1.ScreenHeight);
    font = Game1.font;

    // Load part buttons dynamically from WorldEntityFactory.Previews
    LoadPartButtons();

    // Save button
    openSaveModalButton = new Button(SpriteFactory.GetSprite(ID_SPRITE.BUTTON, Vector2.Zero, 2f), font)
    {
      Text = "Save",
      Position = new Vector2(Game1.ScreenWidth - 200, Game1.ScreenHeight - 80),
    };
    openSaveModalButton.Click += OpenSaveModalButton_Click;
    components.Add(openSaveModalButton);

    // Modal components
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

    AddOpenLinks();
  }

  private void LoadPartButtons()
  {
    foreach (var btn in partButtons)
    {
      components.Remove(btn);
    }
    partButtons.Clear();

    float scale = 3f;
    float xOffset = 50f;
    float buttonDistance = 5f;
    float currentY = 20f;
    int buttonsPerRow = 3;
    int buttonIndex = 0;

    foreach (var kvp in WorldEntityFactory.Previews)
    {
      ID_ENTITY partID = kvp.Key;
      ISprite previewSprite = kvp.Value;

      if (!WorldEntityLoader.HasModule(partID, ID_MODULE.LinkModule)
        || partID == ID_ENTITY.FILLER)
        continue;

      int row = buttonIndex / buttonsPerRow;
      int col = buttonIndex % buttonsPerRow;

      float xPos = Game1.ScreenWidth - xOffset - (SpriteFactory.textures[(int)ID_SPRITE.BUTTON_ENTITY].Width * scale + buttonDistance) * (buttonsPerRow - col);
      float yPos = currentY + row * (SpriteFactory.textures[(int)ID_SPRITE.BUTTON_ENTITY].Height * scale + buttonDistance);

      EntityButton btn = new EntityButton(
        previewSprite,
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale),
        autoFit: true
      )
      {
        Scale = scale,
        Position = new Vector2(xPos, yPos)
      };

      btn.Click += (sender, e) => OnPartButtonClicked(partID, sender as EntityButton);

      if (buttonIndex == 0)
      {
        btn.IsClicked = true;
        clicked = btn;
        idToBeAddded = partID;
      }

      partButtons.Add(btn);
      components.Add(btn);
      buttonIndex++;
    }
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
    await new Birds.src.storage.implementations.JsonBlueprintStorage().SaveBlueprintAsync(blueprint);
    CompositeControllerFactory.InitializePreviews();
    isSaveModalOpen = false;
    textInput.IsActive = false;

    if (previousState is BuildControllerState buildState)
    {
      buildState.LoadBlueprintButtons();
    }
  }

  public override void Update(GameTime gameTime)
  {
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

    base.Update(gameTime);
    Input.HandleZoom();
    editedController.Update(gameTime);

    UpdateClickedState();
    bool mouseAboveComponent = IsMouseAboveComponent();

    if (Input.IsPressed && !mouseAboveComponent)
    {
      var bc = editedEntity.GetModule<BaseCollisionDetectionModule>().BoundingCircle;
      if (bc.Contains(Input.PositionGameCoords))
      {
        AddEntityIfFillerClicked();
      }
      else if (!wasPressed)
      {
        ReturnToPreviousState();
      }
    }
    wasPressed = Input.IsPressed;
  }

  private void UpdateClickedState()
  {
    if (clicked != previouslyClicked)
    {
      if (previouslyClicked != null)
        previouslyClicked.IsClicked = false;
      previouslyClicked = clicked;
      clicked.IsClicked = true;
    }
  }

  private bool IsMouseAboveComponent()
  {
    foreach (IComponent c in components)
    {
      if (c is Button b && b.IsHovering())
      {
        return true;
      }
    }
    return false;
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

  private void ReturnToPreviousState()
  {
    editedController.Entities.Set(Enumerable.Empty<IEntity>());
    editedController.Dispose();
    game.ChangeState(previousState);
    var managementModule = editedEntity.GetModule<LinkManagementModule>();
    managementModule.ClearFillerEntities();
    originalController.Entities.Remove(originalEntity);
    originalEntity.Dispose();
    originalController.Entities.Add(editedEntity);
    Input.Camera.Controller = originalController;
    Input.Camera.Position = originalController.Position;
    Input.Camera.InBuildScreen = true;
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    backgroundState.Draw(gameTime, spriteBatch);
    spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied, samplerState: SamplerState.AnisotropicClamp);
    overlay.Draw(spriteBatch);
    spriteBatch.End();
    spriteBatch.Begin(transformMatrix: Input.Camera.Transform, sortMode: SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, samplerState: SamplerState.AnisotropicClamp);
    editedController.Draw(spriteBatch);
    DrawAvailableLinks(spriteBatch);
    spriteBatch.End();
    base.Draw(gameTime, spriteBatch);

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