using Birds.src.api.client;
using Birds.src.api.server;
using Birds.src.containers.composite;
using Birds.src.factories;
using Birds.src.menu;
using Birds.src.network;
using Birds.src.player;
using Birds.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace Birds.src;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private Input _input;
  public static int ScreenWidth;
  public static int ScreenHeight;
  public static float GRAVITY = 10;
  public static SpriteFont font;
  public static float timeStep = (1f / 60f);
  private IState currentState;
  private IState nextState;
  public static bool LOG_MODULE_PERFORMANCE = false;
  public static bool DRAW_OBB_OUTLINE = false;
  public static bool DRAW_BC_OUTLINE = false;
  public static bool DRAW_AABB_OUTLINE = false;
  public ServerManager ServerManager { get; } = new ServerManager();


  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    IsMouseVisible = true;
  }

  protected override void Initialize()
  {
    RuntimeContext.IsServer = false;
    ClientSession.Initialize();
    _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
    _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
    ScreenWidth = _graphics.PreferredBackBufferWidth;
    ScreenHeight = _graphics.PreferredBackBufferHeight;
    _graphics.PreferMultiSampling = true;
    DrawModule.InitializePixel(GraphicsDevice);
    GRAVITY = 10;
    base.Initialize();
  }

  protected override void LoadContent()
  { // use this and Content to load your game content here
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _graphics.ApplyChanges();

    SpriteFactory.LoadTextures(Content);

    font = Content.Load<SpriteFont>("menu/FONT");
    SpriteLoader.Initialize();
    WorldEntityFactory.InitializePreviews();
    CompositeControllerFactory.InitializePreviews();
    WarmupPropertyCache();
    _input = new Input(
      InputConfiguration.LoadDefault(),
      TouchPanel.GetCapabilities().IsConnected
        ? new TouchDevice()
        : new MouseDevice()
    );
    currentState = new MainMenu(this, GraphicsDevice, Content, _input);
  }

  //Im not sure i like this but it does improve things significantly since we are compiling syncing
  private void WarmupPropertyCache()
  {
    var dummyEntity = WorldEntityFactory.GetEntity(Vector2.Zero, ID_ENTITY.DEFAULT, false);
    dummyEntity.Update(new GameTime());
    dummyEntity.Dispose();
    var dummyComposite = CompositeControllerFactory.CreateComposites(Vector2.Zero, 1, CompositeControllerFactory.DEFAULT_SINGLE)[0] as CompositeController;
    dummyComposite?.Update(new GameTime());
    dummyComposite?.Dispose();
    var dummyController = ControllerFactory.Create(
        WorldEntityFactory.CreateEntities(Vector2.Zero, 1, ID_ENTITY.DEFAULT),
        ID_CONTROLLER.DEFAULT
    );
    dummyController.Update(new GameTime());
  }

  public void ChangeState(IState state)
  {
    nextState = state;
  }

  public IState GetNextState()
  {
    if (nextState == null)
      return currentState;
    return nextState;
  }

  protected override void Update(GameTime gameTime)
  {
    _input.Update(gameTime);
    currentState.Update(gameTime);
    currentState.PostUpdate();
    if (LOG_MODULE_PERFORMANCE)
    {
      ModuleProfiler.Summary();
    }
    if (nextState != null)
    {
      currentState = nextState;
      nextState = null;
    }
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
    ServerManager.StopLocalServer();
    base.OnExiting(sender, args);
  }
}