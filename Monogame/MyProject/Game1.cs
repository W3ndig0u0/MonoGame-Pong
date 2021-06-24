using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MyProject
{
  public class Game1 : Game
  {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D doubleBuffer;
    private Rectangle renderRectangle;
    private Texture2D _texture;

    private Paddle[] paddles;
    private int[] scores;
    private SpriteFont font;
    private Ball ball;

    private bool lastPointSide = true;
    private readonly Random rand;

    public enum GameState { Idle, Start, Play, CheckEnd }
    private GameState gameState;

    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
      this.TargetElapsedTime = new TimeSpan(33333);
      Window.AllowUserResizing = true;

      gameState = GameState.Idle;
      rand = new Random();

      paddles = new Paddle[2];
    }

    protected override void Initialize()
    {
      // TODO: Add your initialization logic here
      // !Storlek
      doubleBuffer = new RenderTarget2D(GraphicsDevice, 640, 480);
      _graphics.PreferredBackBufferWidth = 640;
      _graphics.PreferredBackBufferHeight = 480;

      _graphics.IsFullScreen = false;

      // !Applt annars ändras det inte!
      _graphics.ApplyChanges();

      Window.ClientSizeChanged += OnWindowSizeChange;
      OnWindowSizeChange(null, null);

      ball = new Ball(rand, lastPointSide);

      base.Initialize();
    }
    protected override void LoadContent()
    {
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
      _texture = new Texture2D(GraphicsDevice, 1, 1);
      Color[] data = new Color[1];
      data[0] = Color.White;
      _texture.SetData(data);

      font = Content.Load<SpriteFont>("font");
    }

    protected override void Update(GameTime gameTime)
    {
      // TODO: Add your update logic here
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // !Game State

      switch (gameState)
      {
        case GameState.Idle:
          ball.Move(true);
          gameState = GameState.Start;
          break;

        case GameState.Start:
          ball = new Ball(rand, lastPointSide);
          paddles[0] = new Paddle(false);
          paddles[1] = new Paddle(true);
          scores = new int[2];
          gameState = GameState.Play;
          break;

        case GameState.Play:
          (int scored, bool bounced) = ball.Move(false);

          paddles[0].AIMove(ball);
          paddles[1].AIMove(ball);

          var hit = paddles[0].CollisionCheck(ball);
          hit |= paddles[1].CollisionCheck(ball);

          if (hit) return;

          if (scored == 0) return;

          gameState = GameState.CheckEnd;
          lastPointSide = scored == 1;
          int index = lastPointSide ? 0 : 1;
          scores[index]++;

          break;

        case GameState.CheckEnd:
          ball = new Ball(rand, lastPointSide);
          if (scores[0] > 9 || scores[1] > 9)
          {
            gameState = GameState.Idle;
            return;
          }

          gameState = GameState.Play;
          break;

        default:
          gameState = GameState.Idle;
          break;
      }

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

      // TODO: Add your drawing code here
      GraphicsDevice.SetRenderTarget(doubleBuffer);
      GraphicsDevice.Clear(Color.Black);
      for (int i = 0; i < 31; i++)
      {
        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, new Rectangle(doubleBuffer.Width / 2, i * doubleBuffer.Height / 31, 2, doubleBuffer.Height / 62), Color.White);

        switch (gameState)
        {
          case GameState.Idle:
            _spriteBatch.Draw(_texture, ball.box, Color.White);
            break;
          case GameState.Start:
            break;

          case GameState.Play:
          case GameState.CheckEnd:
            _spriteBatch.Draw(_texture, ball.box, Color.White);

            _spriteBatch.Draw(_texture, paddles[0].box, Color.White);
            _spriteBatch.Draw(_texture, paddles[1].box, Color.White);

            // _spriteBatch.DrawString(font, scores[0].ToString(), new Vector2(64, 0), Color.White);
            // _spriteBatch.DrawString(font, scores[1].ToString(), new Vector2(doubleBuffer.Width - 10, 10), Color.White);
            break;

          default:
            Console.WriteLine("Draw Default error");
            break;
        }

        _spriteBatch.End();
      }

      GraphicsDevice.SetRenderTarget(null);
      GraphicsDevice.Clear(Color.CornflowerBlue);

      _spriteBatch.Begin();
      _spriteBatch.Draw(doubleBuffer, renderRectangle, Color.White);
      _spriteBatch.End();

      base.Draw(gameTime);
    }

    // !Game Size
    void OnWindowSizeChange(object sender, EventArgs e)
    {
      // !Game saze resolution when resizing game
      float width = Window.ClientBounds.Width;
      float height = Window.ClientBounds.Height;

      if (height < width / (float)doubleBuffer.Width * doubleBuffer.Height)
      {
        width = (int)(height / (float)doubleBuffer.Width * doubleBuffer.Height);
      }
      else
        height = (int)(width / (float)doubleBuffer.Width * doubleBuffer.Height);

      int x = (int)(Window.ClientBounds.Width - width) / 2;
      int y = (int)(Window.ClientBounds.Height - height) / 2;
      renderRectangle = new Rectangle(x, y, (int)width, (int)height);
    }
  }
}
