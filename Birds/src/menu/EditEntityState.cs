using Birds.src.containers.entity;
using Birds.src.factories;
using Birds.src.menu.controls;
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
    Input.Camera.Controller = this.entityEdited;
    this.entityEdited = entityEdited;
    //idToBeAddded = IDs.COMPOSITE;
    float scale = 3;

    #region AddingButtons
    EntityButton addRectangularHullButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.HULL_RECTANGULAR, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale),
        true)
      {
        Scale = scale,
        Position = new Vector2(Game1.ScreenWidth - SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR].Width * scale - 100, 20),
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
        Position = new Vector2(addRectangularHullButton.Position.X - SpriteFactory.textures[(int)ID_SPRITE.HULL_CIRCULAR].Width * scale, 20),
      };
    addCircularHullButton.Click += AddCircularHullButton_Click;

    EntityButton addLinkHullButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.HULL_LINK, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale),
        true)
      {
        Scale = scale,
        Position = new Vector2(addCircularHullButton.Position.X - SpriteFactory.textures[(int)ID_SPRITE.HULL_LINK].Width * scale, 20),
      };
    addLinkHullButton.Click += AddLinkHullButton_Click;

    EntityButton addEngineButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.ENGINE, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale))
      {
        Scale = scale,
        Position = new Vector2(Game1.ScreenWidth - SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR].Width * scale - 100, 5 + addRectangularHullButton.Position.Y + addRectangularHullButton.Rectangle.Height),
      };
    addEngineButton.Click += AddEngineButton_Click;

    EntityButton addShooterButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.GUN, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale))
      {
        Scale = scale,
        Position = new Vector2(Game1.ScreenWidth - SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR].Width * scale - 100, 5 + addEngineButton.Position.Y + addEngineButton.Rectangle.Height),
      };
    addShooterButton.Click += AddShooterButton_Click;

    EntityButton addSpikeButton =
      new EntityButton(
        SpriteFactory.GetSprite(ID_SPRITE.SPIKE, Vector2.Zero, scale),
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale))
      {
        Scale = scale,
        Position = new Vector2(Game1.ScreenWidth - SpriteFactory.textures[(int)ID_SPRITE.HULL_RECTANGULAR].Width * scale - 100, 5 + addShooterButton.Position.Y + addShooterButton.Rectangle.Height),
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
    base.Update(gameTime);/*
    if (clicked != previouslyClicked)
    {
      if (previouslyClicked != null)
        previouslyClicked.IsClicked = false;
      previouslyClicked = clicked;
      clicked.IsClicked = true;
    }
    bool interactWithMenuController = true;
    foreach (IComponent c in components)
      if (c is Button b && b.MouseIntersects())
        interactWithMenuController = false;
    if (interactWithMenuController)
    {
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
      }
    }
    else
    {
      menuController.newClickRequired = true;
      menuController.clickedOutside = false;
      menuController.removeEntity = false;
      menuController.clickedOnControllable = false;
    }
    if (input.BuildClicked)
    {
      /*menuController.ClearOpenLinks();
      buildOverviewState.menuController.Remove(entityEdited);
      foreach (IControllable c in menuController.Controllables)
      {

          buildOverviewState.menuController.AddControllable(c);

      }*//*
      menuController.DeFocus();
      previousState.BuildClicked();
    }*/
  }
}
