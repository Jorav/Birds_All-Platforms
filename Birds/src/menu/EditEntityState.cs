using Birds.src.containers.composite;
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
using System.Threading.Tasks;

namespace Birds.src.menu;

public class EditEntityState : BuildStateBase
{
  private IEntity editedEntity;
  private IEntity originalEntity;
  private ID_ENTITY idToBeAddded;
  private EntityButton clicked;
  private EntityButton previouslyClicked;
  private EntityButtonManager buttonManager;
  private EntityButton saveButton;
  private SpriteFont font;
  private bool saveAndExit;
  private bool refreshFillers = false;

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
    AddOpenLinks();
    Input.Camera.Controller = editedController;
    Input.Camera.InBuildScreen = true;
  }

  private void InitializeButtons()
  {
    buttonManager = new EntityButtonManager(components);
    LoadPartButtons();

    ISprite saveIcon = SpriteFactory.GetSprite(ID_SPRITE.SAVE, Vector2.Zero);
    ISprite buttonBg = SpriteFactory.GetSprite(ID_SPRITE.ENTITY_BUTTON_MENU, Vector2.Zero);

    saveButton = new EntityButton(saveIcon, buttonBg, autoFit: true)
    {
      Scale = 4f,
      Position = new Vector2(Game1.ScreenWidth - 200, Game1.ScreenHeight - 200),
    };

    saveButton.Click += SaveButton_Click;
    components.Add(saveButton);
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

  private void OnPartButtonClicked(ID_ENTITY partID, EntityButton clickedButton)
  {
    idToBeAddded = partID;
    clicked = clickedButton; AddOpenLinks();
    refreshFillers = true;
  }

  private void SaveButton_Click(object sender, EventArgs e)
  {
    saveAndExit = true;
  }

  private async Task SaveEntityAndUpdatePreviousState()
  {
    var managementModule = editedEntity.GetModule<LinkManagementModule>();
    managementModule.ClearFillerEntities();

    var entitiesToSave = editedEntity.Entities.Cast<WorldEntity>().ToList();
    string newName = Guid.NewGuid().ToString();

    var blueprint = BlueprintFactory.CreateBlueprint(entitiesToSave, newName);
    await BlueprintFactory.SaveBlueprintAsync(blueprint);
    await CompositeControllerFactory.InitializePreviews();

    if (backgroundState is BuildControllerState buildState)
    {
      buildState.LoadBlueprintButtons();
    }

    ReturnToPreviousState();
  }

  public override void Update(GameTime gameTime)
  {
    base.Update(gameTime);
    if (refreshFillers)
    {
      AddOpenLinks();
      refreshFillers = false;
    }
    saveButton?.Update(gameTime);

    if (saveAndExit)
    {
      _ = SaveEntityAndUpdatePreviousState();
      saveAndExit = false;
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
      else if (Input.WasJustPressed)
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
          refreshFillers = true;
        }
        break;
      }
    }
  }

  private void AddOpenLinks()
  {
    var managementModule = editedEntity.GetModule<LinkManagementModule>();
    managementModule.AddFillerEntities(idToBeAddded);
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
    // Save button is drawn via the components list in the base class
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