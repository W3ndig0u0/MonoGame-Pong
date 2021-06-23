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

    // !Paddle
    private Rectangle[] paddle;

    // !Ball
    private Rectangle ball;
    private Point ballVelocity;
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
    }

    protected override void Initialize()
    {
      // TODO: Add your initialization logic here
      // !Storlek
      doubleBuffer = new RenderTarget2D(GraphicsDevice, 720, 480);
      _graphics.PreferredBackBufferHeight = 720;
      _graphics.PreferredBackBufferWidth = 720;

      _graphics.IsFullScreen = false;

      // !Applt annars ändras det inte!
      _graphics.ApplyChanges();

      Window.ClientSizeChanged += OnWindowSizeChange;
      OnWindowSizeChange(null, null);
      ResetBall();

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
          MoveBall(true);
          // gameState = GameState.Start;
          break;

        case GameState.Start:
          ResetBall();
          gameState = GameState.Play;
          break;

        case GameState.Play:
          int scored = MoveBall(false);

          if (scored == 1)
          {
            lastPointSide = true;
            gameState = GameState.CheckEnd;
          }

          else if (scored == 1)
          {
            lastPointSide = false;
            gameState = GameState.CheckEnd;
          }
          break;

        case GameState.CheckEnd:
          ResetBall();
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
            _spriteBatch.Draw(_texture, ball, Color.White);
            break;
          case GameState.Start:
            break;

          case GameState.Play:
          case GameState.CheckEnd:
            _spriteBatch.Draw(_texture, ball, Color.White);
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

    // !Paddle
    private void AIPaddle(int index)
    {
      int delta = ball.Y + ball.Height / 2 - (paddle[index].Y + paddle[index].Height / 2);
      paddle[index].Y += delta;
    }

    // !Ball
    private void ResetBall()
    {
      int minSpeed = 2;
      int maxSpeed = 7;

      ball = new Rectangle(doubleBuffer.Width / 2 - 4, doubleBuffer.Height / 2 - 4, 8, 8);
      ballVelocity = new Point(lastPointSide ? rand.Next(minSpeed, maxSpeed) : -rand.Next(minSpeed, maxSpeed), rand.Next() > int.MaxValue ? rand.Next(minSpeed, maxSpeed) : -rand.Next(minSpeed, maxSpeed));
    }

    public int MoveBall(bool bounce)
    {
      ball.X += ballVelocity.X;
      ball.Y += ballVelocity.Y;

      if (ball.Y < 0)
      {
        ball.Y = -ball.Y;
        ballVelocity.Y = -ballVelocity.Y;
      }

      if (ball.Y + ball.Height > doubleBuffer.Height)
      {
        ball.Y = doubleBuffer.Height - ball.Height - (ball.Y + ball.Height - doubleBuffer.Height);
        ballVelocity.Y = -ballVelocity.Y;
      }

      if (ball.X < 0)
      {
        if (bounce)
        {
          ball.X = -ball.X;
          ballVelocity.X = -ballVelocity.X;
        }
        else return -1;
      }

      if (ball.X + ball.Width > doubleBuffer.Width)
      {
        if (bounce)
        {
          ball.X = doubleBuffer.Width - ball.Width - (ball.X + ball.Width - doubleBuffer.Width);
          ballVelocity.X = -ballVelocity.X;
        }
        else return 1;
      }
      return 0;
    }

    // !Game Size
    private void OnWindowSizeChange(object sender, EventArgs e)
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
