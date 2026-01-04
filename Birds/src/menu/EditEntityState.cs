using Birds.src.containers.entity;
using Birds.src.events;
using Birds.src.factories;
using Birds.src.menu.controls;
using Birds.src.modules.composite;
using Birds.src.modules.entity;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Birds.src.menu;

public class EditEntityState : MenuState
{
  State previousState;
  IEntity entityEdited;
  ID_ENTITY idToBeAddded;
  EntityButton clicked;
  EntityButton previouslyClicked;

  public EditEntityState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, State previousState, Input input, IEntity entityEdited) : base(game, graphicsDevice, content, input)
  {
    this.previousState = previousState;
    components = new List<IComponent>();
    this.entityEdited = entityEdited;
    Input.Camera.Controller = this.entityEdited;
    Input.Camera.Zoom = Input.Camera.BuildMenuZoom;
    Input.Camera.InBuildScreen = true;
    idToBeAddded = ID_ENTITY.DEFAULT;
    float scale = 3f;
    float xOffset = 50f;
    float buttonDistance = 5f;
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
    entityEdited.Update(gameTime);
    if (clicked != previouslyClicked)
    {
      if (previouslyClicked != null)
        previouslyClicked.IsClicked = false;
      previouslyClicked = clicked;
      clicked.IsClicked = true;
    }
    bool interactWithMenuController = true;
    foreach (IComponent c in components)
      if (c is Button b && b.IsHovering())
        interactWithMenuController = false;
    if (interactWithMenuController)
    {/*
      if (menuController.clickedOnControllable)
      {
        IControllable clickedC = menuController.controllableClicked;
        if (clickedC is WorldEntity clickedE && clickedE.IsFiller)
        {
          menuController.ReplaceEntity(clickedE, EntityFactory.Create(menuController.Position, idToBeAddded));
        }
        menuController.clickedOnControllable = false;
      }
      if (menuController.removeEntity)
      {
        IControllable clickedC = menuController.controllableClicked;
        if (clickedC is WorldEntity clickedE && !clickedE.IsFiller)
        {
          menuController.RemoveEntity(clickedE);
        }
        menuController.removeEntity = false;
        //menuController.requireNewClick = true;
        //menuController.clickedOutside = true;
      }
      if (menuController.clickedOutside)
      {
        menuController.DeFocus();
        previousState.previousScrollValue = previousScrollValue;
        previousState.currentScrollValue = currentScrollValue;
        game.ChangeState(previousState);
        menuController.clickedOutside = false;
      }*/
    }
    else
    {/*
      menuController.newClickRequired = true;
      menuController.clickedOutside = false;
      menuController.removeEntity = false;
      menuController.clickedOnControllable = false;*/
    }
    if (input.BuildClicked)
    {
      /*menuController.ClearOpenLinks();
      buildOverviewState.menuController.Remove(entityEdited);
      foreach (IControllable c in menuController.Controllables)
      {

          buildOverviewState.menuController.AddControllable(c);

      }
      menuController.DeFocus();
      previousState.BuildClicked();*/
    }
  }

  private void AddOpenLinks()
  {
    var managementModule = entityEdited.GetModule<LinkManagementModule>();
    managementModule.AddFillerEntities();
  }

  public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
  {
    previousState.Draw(gameTime, spriteBatch);

    spriteBatch.Begin(transformMatrix: Input.Camera.Transform, sortMode: SpriteSortMode.Deferred, blendState: BlendState.AlphaBlend, samplerState: SamplerState.AnisotropicClamp);
    entityEdited.Draw(spriteBatch);
    DrawAvailableLinks(spriteBatch);
    spriteBatch.End();

    base.Draw(gameTime, spriteBatch);
  }

  private void DrawAvailableLinks(SpriteBatch spriteBatch)
  {
    var pixelTexture = new Texture2D(graphicsDevice, 1, 1);
    pixelTexture.SetData(new[] { Color.Green });
    foreach (IEntity entity in entityEdited.Entities)
    {
      var linkModule = entity.GetModule<LinkModule>();
      foreach (var link in linkModule.Links)
      {
        if (link.ConnectionAvailable)
        {
          spriteBatch.Draw(pixelTexture, new Rectangle((int)link.AbsolutePosition.X - 2, (int)link.AbsolutePosition.Y - 2, 4, 4), Color.Yellow);
          spriteBatch.Draw(pixelTexture, new Rectangle((int)link.ConnectionPosition.X - 2, (int)link.ConnectionPosition.Y - 2, 4, 4), Color.Blue);
        }
      }
    }
  }
}
