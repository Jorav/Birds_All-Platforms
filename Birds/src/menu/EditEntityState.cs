using Birds.src.containers.controller;
using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.menu.controls;
using Birds.src.modules.composite;
using Birds.src.modules.entity;
using Birds.src.modules.shared.collision_detection;
using Birds.src.utility;
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
    float scale = 3f;
    float xOffset = 50f;
    float buttonDistance = 5f;
    overlay = SpriteFactory.GetSprite(ID_SPRITE.BACKGROUND_WHITE, new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2), SpriteFactory.textures[(int)ID_SPRITE.BACKGROUND_WHITE].Height / Game1.ScreenHeight);

    #region AddingButtons
    EntityButton addRectangularHullButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.HULL_RECTANGULAR, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale),
        true)
      {
        Scale = scale,
        Position = new Vector2(Game1.ScreenWidth - SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR].Width * scale - xOffset, 20),
      };
    addRectangularHullButton.Click += AddRectangularHullButton_Click;
    addRectangularHullButton.IsClicked = true;
    clicked = addRectangularHullButton;

    EntityButton addCircularHullButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.HULL_CIRCULAR, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale),
        true)
      {
        Scale = scale,
        Position = new Vector2(addRectangularHullButton.Position.X - addRectangularHullButton.entitySprite.Width * scale - buttonDistance, 20),
      };
    addCircularHullButton.Click += AddCircularHullButton_Click;

    EntityButton addLinkHullButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.HULL_LINK, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale),
        true)
      {
        Scale = scale,
        Position = new Vector2(addCircularHullButton.Position.X - addCircularHullButton.entitySprite.Width * scale - buttonDistance, 20),
      };
    addLinkHullButton.Click += AddLinkHullButton_Click;

    EntityButton addEngineButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.ENGINE, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale))
      {
        Scale = scale,
        Position = new Vector2(Game1.ScreenWidth - SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR].Width * scale - xOffset, buttonDistance + addRectangularHullButton.Position.Y + addRectangularHullButton.Rectangle.Height),
      };
    addEngineButton.Click += AddEngineButton_Click;

    EntityButton addShooterButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.GUN, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale))
      {
        Scale = scale,
        Position = new Vector2(Game1.ScreenWidth - SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR].Width * scale - xOffset, buttonDistance + addEngineButton.Position.Y + addEngineButton.Rectangle.Height),
      };
    addShooterButton.Click += AddShooterButton_Click;

    EntityButton addSpikeButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.SPIKE, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale))
      {
        Scale = scale,
        Position = new Vector2(Game1.ScreenWidth - SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR].Width * scale - xOffset, buttonDistance + addShooterButton.Position.Y + addShooterButton.Rectangle.Height),
      };
    addSpikeButton.Click += AddSpikeButton_Click;
    #endregion

    components = new List<IComponent>()
    {
      addRectangularHullButton,
      addCircularHullButton,
      addLinkHullButton,
      addEngineButton,
      addShooterButton,
      addSpikeButton,
    };
    AddOpenLinks();
  }

  #region OnClicks
  private void AddEngineButton_Click(object sender, EventArgs e)
  {
    idToBeAddded = ID_ENTITY.ENGINE;
    clicked = (EntityButton)sender;
  }

  private void AddLinkHullButton_Click(object sender, EventArgs e)
  {
    idToBeAddded = ID_ENTITY.LINK_COMPOSITE;
    clicked = (EntityButton)sender;
  }

  private void AddCircularHullButton_Click(object sender, EventArgs e)
  {
    idToBeAddded = ID_ENTITY.CIRCULAR;
    clicked = (EntityButton)sender;
  }

  private void AddSpikeButton_Click(object sender, EventArgs e)
  {
    idToBeAddded = ID_ENTITY.SPIKE;
    clicked = (EntityButton)sender;
  }

  private void AddRectangularHullButton_Click(object sender, EventArgs e)
  {
    idToBeAddded = ID_ENTITY.DEFAULT;
    clicked = (EntityButton)sender;
  }

  private void AddShooterButton_Click(object sender, EventArgs e)
  {
    idToBeAddded = ID_ENTITY.SHOOTER;
    clicked = (EntityButton)sender;
  }
  #endregion

  public override void Update(GameTime gameTime)
  {
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
      else if(!wasPressed)
      {
        ReturnToPreviousState();
      }
    }
    if (input.BuildClicked)
    {
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
        bool succefullyReplaced = editedEntity.ReplaceEntity(entity, WorldEntityFactory.CreateEntities(entity.Position, 1, idToBeAddded, isComposite: true).First());
        if (succefullyReplaced)
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
  }

  private void DrawAvailableLinks(SpriteBatch spriteBatch)
  {
    var pixelTexture = new Texture2D(graphicsDevice, 1, 1);
    pixelTexture.SetData(new[] { Color.Green });
    foreach (IEntity entity in editedEntity.Entities)
    {
      var linkModule = entity.GetModule<LinkModule>();
      foreach (var link in linkModule.Links)
      {
        if (link.ConnectionAvailable)
        {
          spriteBatch.Draw(pixelTexture, new Rectangle((int)link.AbsolutePositionOnEntity.X - 2, (int)link.AbsolutePositionOnEntity.Y - 2, 4, 4), Color.Yellow);
          spriteBatch.Draw(pixelTexture, new Rectangle((int)link.ConnectionPosition.X - 2, (int)link.ConnectionPosition.Y - 2, 4, 4), Color.Blue);
        }
      }
    }
  }
}
