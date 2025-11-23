using Birds.src.factories;
using Birds.src.menu;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Birds.src;
public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  //private PerformanceMeasurer performanceMeasurer;
  //private MeanSquareError meanSquareError;
  public static int ScreenWidth;
  public static int ScreenHeight;
  public static float GRAVITY = 10;
  public static SpriteFont font;
  public static float timeStep = (1f / 60f);
  private State currentState;
  private State nextState;
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
  }

  protected override void Initialize()
  {
    //Add your initialization logic here
    _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
    _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
    ScreenWidth = _graphics.PreferredBackBufferWidth;
    ScreenHeight = _graphics.PreferredBackBufferHeight;
    _graphics.PreferMultiSampling = true;
    DrawModule.InitializePixel(GraphicsDevice);
    base.Initialize();
  }

  protected override void LoadContent()
  { // use this and Content to load your game content here
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _graphics.ApplyChanges();
    Input input = new Input()
    {
      Up = Keys.W,
      Down = Keys.S,
      Left = Keys.A,
      Right = Keys.D,
      Pause = Keys.Escape,
      Build = Keys.B,
      Enter = Keys.Enter,
    };
    GRAVITY = 10;
    Texture2D[] textures = new Texture2D[Enum.GetNames(typeof(ID_SPRITE)).Length];
    textures[(int)ID_SPRITE.HULL_RECTANGULAR] = Content.Load<Texture2D>("parts/HULL_RECTANGULAR");
    textures[(int)ID_SPRITE.HULL_CIRCULAR] = Content.Load<Texture2D>("parts/HULL_CIRCULAR");
    textures[(int)ID_SPRITE.HULL_LINK] = Content.Load<Texture2D>("parts/HULL_LINK");
    textures[(int)ID_SPRITE.ENGINE] = Content.Load<Texture2D>("parts/ENGINE");
    textures[(int)ID_SPRITE.GUN] = Content.Load<Texture2D>("parts/GUN");
    textures[(int)ID_SPRITE.PART_EMPTY] = Content.Load<Texture2D>("parts/ENGINE");
    textures[(int)ID_SPRITE.SPIKE] = Content.Load<Texture2D>("parts/SPIKE");
    textures[(int)ID_SPRITE.CLOUD] = Content.Load<Texture2D>("background/CLOUD");
    textures[(int)ID_SPRITE.SUN] = Content.Load<Texture2D>("background/SUN");
    textures[(int)ID_SPRITE.BACKGROUND_WHITE] = Content.Load<Texture2D>("background/WHITE_SMALL");
    textures[(int)ID_SPRITE.BACKGROUND_GRAY] = Content.Load<Texture2D>("background/GRAY");
    textures[(int)ID_SPRITE.BUTTON] = Content.Load<Texture2D>("menu/BUTTON");
    textures[(int)ID_SPRITE.BUTTON_ENTITY] = Content.Load<Texture2D>("menu/BUTTON_ENTITY");

    SpriteFactory.textures = textures;
    font = Content.Load<SpriteFont>("menu/FONT");


    currentState = new MainMenu(this, GraphicsDevice, Content, input);
  }
  public void ChangeState(State state)
  {
    nextState = state;
  }
  public State GetNextState()
  {
    if (nextState == null)
      return currentState;
    return nextState;
  }

  protected override void Update(GameTime gameTime)
  {
    if (nextState != null)
    {
      currentState = nextState;
      nextState = null;
    }
    Input.Update(gameTime);
    currentState.Update(gameTime);
    currentState.PostUpdate();
    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    ScreenWidth = _graphics.PreferredBackBufferWidth;
    ScreenHeight = _graphics.PreferredBackBufferHeight;
    currentState.Draw(gameTime, _spriteBatch);
    base.Draw(gameTime);
  }

  protected override void OnExiting(object sender, EventArgs args)
  {
    //SaveGame();
    base.OnExiting(sender, args);
  }
}

