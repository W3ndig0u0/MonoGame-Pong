using System;
using Microsoft.Xna.Framework;

namespace MyProject
{
  public class Ball
  {

    public Rectangle box
    {
      get; private set;
    }
    public Point Velocity
    {
      get; private set;
    }

    public Ball(Random rand, bool dir)
    {
      box = new Rectangle(640 / 2, 480 / 2, 8, 8);
      Velocity = new Point(dir ? rand.Next(1, 2) : -rand.Next(1, 2),
      rand.Next() > int.MaxValue / 2 ? rand.Next(1, 2) : -rand.Next(1, 2));
    }

    public (int, bool) Move(bool bounceOffSides)
    {
      bool bounced = false;
      int score = 0;

      var pos = box.Location;

      pos.X += Velocity.X;
      pos.Y += Velocity.Y;

      if (pos.Y < 0)
      {
        bounced = true;
        pos.Y = -pos.Y;
        ReverseVelocity(y: true);
      }

      if (pos.Y + box.Height > 480)
      {
        bounced = true;
        pos.Y = 480 - (pos.Y + box.Height - 480);
        ReverseVelocity(y: true);
      }

      if (pos.X < 0)
      {
        if (bounceOffSides)
        {
          bounced = true;
          pos.X = 0;
          ReverseVelocity(x: true);
        }
        else score = -1;
      }

      if (pos.X + box.Width > 640)
      {
        if (bounceOffSides)
        {
          bounced = true;
          pos.Y = 480 - (pos.Y + box.Height - 480);
          ReverseVelocity(x: true);
        }
        else score = 1;
      }

      SetPosition(pos);
      return (score, bounced);
    }

    public void SetPosition(Point point)
    {
      box = new Rectangle(point, box.Size);
    }

    public void ReverseVelocity(bool x = false, bool y = false)
    {
      var vel = Velocity;

      if (x) vel.X = -vel.X;
      if (y) vel.Y = -vel.Y;

      Velocity = vel;
    }
  }
}
