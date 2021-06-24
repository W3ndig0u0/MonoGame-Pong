using System;
using Microsoft.Xna.Framework;

namespace MyProject
{
  public class Paddle
  {

    public Rectangle box
    {
      get; private set;
    }
    readonly bool _side;

    public Paddle(bool side)
    {
      _side = side;
      var x = side ? 600 : 32;
      box = new Rectangle(new Point(x, 224), new Point(8, 32));
    }

    public bool BallIsAbleToHit(Ball ball)
    {
      bool dirCheck;
      bool disCheck;
      if (_side)
      {
        dirCheck = ball.Velocity.X > 0;
        disCheck = ball.box.X + ball.box.Width > box.X;
        return dirCheck & disCheck;
      }

      dirCheck = ball.Velocity.X < 0;
      disCheck = ball.box.X < ball.box.Width + box.X;
      return dirCheck & disCheck;
    }


    public bool PaddleCheck(int x, int y, Ball ball)
    {
      return x <= box.X + box.Width &&
       x + ball.box.Width >= box.X &&
       y <= box.Y + box.Height &&
       y + ball.box.Height >= box.Y;
    }

    public bool CollisionCheck(Ball ball)
    {
      if (!BallIsAbleToHit(ball)) return false;

      (float delta, bool wayPastPaddle) = FindDeltaBallMovement(ball);

      if (wayPastPaddle) return false;


      float deltaTime = delta / ball.Velocity.X;
      int collY = (int)(ball.box.Y - ball.Velocity.Y * deltaTime);
      int collX = (int)(ball.box.X - ball.Velocity.X * deltaTime);

      if (PaddleCheck(collX, collY, ball))
      {
        ball.SetPosition(new Point(collX, collY));

        var diffY = (collY + ball.box.Height / 2) - (box.Y + box.Height / 2);
        diffY /= box.Height / 8;
        diffY -= Math.Sign(diffY);

        // ball.IncreaseVelocity(Math.Sign(ball.Velocity.X), diffY);
        ball.ReverseVelocity(true);

        return true;
      }

      return false;
    }
    private void FixBounds(Point pos)
    {
      if (pos.Y < box.Height) pos.Y = box.Height;
      if (pos.Y + box.Height > 480) pos.Y = 480 - box.Height;

      box = new Rectangle(pos, box.Size);
    }
    public (float, bool) FindDeltaBallMovement(Ball ball)
    {
      float delta;
      bool wayPastPaddle;

      if (_side)
      {
        delta = ball.box.X + ball.box.Width - box.X;
        wayPastPaddle = ball.box.X + ball.box.Width > box.X;
        return (delta, wayPastPaddle);
      }

      delta = ball.box.X - (box.Width + box.X);
      wayPastPaddle = delta < ball.Velocity.X;
      return (delta, wayPastPaddle);
    }

    public static int AiPaddleSpeed = 4;
    public void AIMove(Ball ball)
    {
      var delta = ball.box.Y + ball.box.Height / 2 - (box.Y + box.Height / 2);
      var pos = box.Location;

      if (Math.Abs(delta) > AiPaddleSpeed) delta = Math.Sign(delta) * AiPaddleSpeed;
      pos.Y += delta;
      FixBounds(pos);
    }
    public void PlayerMove(int diff)
    {
      var pos = box.Location;
      pos.Y += diff;
      FixBounds(pos);
    }
  }
}
