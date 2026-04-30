using Data;

namespace Logic;

/// <summary>
/// Class not aware of the mess with [0, 1] and [0, aspectRatio]
/// Assumes the ball is a circle, therefore ALL values should come prescaled to [0, aspectRatio] on X
/// </summary>
internal class Trajectory
{
    private readonly IBall ball;

    public double StartingX { get; private set; }
    public double StartingY { get; private set; }

    public double EndingX { get; private set; }
    public double EndingY { get; private set; }
    public double DiffX { get; private set; }
    public double DiffY { get; private set; }

    public double Radius { get; private set; }

    public Trajectory(IBall ball, double deltaTime)
    {
        StartingX = ball.X;
        StartingY = ball.Y;
        DiffX = ball.Velocity.X * deltaTime;
        DiffY = ball.Velocity.Y * deltaTime;
        EndingX = StartingX + DiffX;
        EndingY = StartingY + DiffY;
        Radius = ball.Radius;
        this.ball = ball;
    }


    /// <summary>
    /// Find the collision point of the trajectory with a half-plane defined by
    /// ax + by + c >= 0
    /// 
    /// Returning -1 if there is no collision is a little scetchy,
    /// but considering the constraints on the ball's movement, it should be fine.
    /// Does not account for ball radious
    /// (balls center needs to be on the straight in the moment of collision, not the edge of the ball)
    /// </summary>
    /// <param name="a" x coefficient></param>
    /// <param name="b" y coefficient></param>
    /// <param name="c" free term></param>
    /// <returns>Progress t along the trajectory from Start to End</returns>
    public double CollideWithHalfPlane(double a, double b, double c)
    {
        // Not comparing with zero to allow numerical rounding errors to pass
        // does create edge cases where the value returned by this method could be slightly negative
        if (a * StartingX + b * StartingY + c > 0.000001)
        {
            throw new ArgumentException($"The trajectory starts in the half-plane, which is an unintended edge case. StartingPoint = ({StartingX}, {StartingY}), half plane = ({a}x + {b}y + {c} >= 0)");
        }

        if (a * EndingX + b * EndingY + c < 0)
        {
            // The trajectory does not end in the half-plane,
            // so, since the trajectory is a straight line, it does not collide with the half-plane at all.
            return -1;
        }

        double d1 = a * StartingX + b * StartingY + c;
        double d2 = a * EndingX + b * EndingY + c;
        return d1 / (d1 - d2);
    }

 

    public double CollideWithBall(IBall otherBall)
    {
        // The trajectory is a straight line, so we can find the distance from the center of the ball to the line,
        // and if it is less than the sum of the ball's radious and the trajectory's radious, then they could collide.
        double a = DiffY;
        double b = -DiffX;
        double c = EndingX * StartingY - StartingX * EndingY;

        double radiusSum = Radius + otherBall.Radius;

        double distance = Math.Abs(a * otherBall.X + b * otherBall.Y + c) / Math.Sqrt(a * a + b * b);
        if (distance > radiusSum)
        {
            // The trajectory does not collide with the ball at all.
            return -1;
        }


        // closest point on the straight extending from the trajectory to the ball
        double t = (-b * (otherBall.X - StartingX)  + a * (otherBall.Y - StartingY)) / (a * a + b * b);

        double dt = Math.Sqrt((radiusSum * radiusSum - distance * distance) / (DiffX * DiffX + DiffY * DiffY));

        double collisionT1 = t - dt;
        double collisionT2 = t + dt;

        // if the collision point closer to the starting ball position if valid, return it
        if (collisionT1 >= 0.00001 && collisionT1 <= 1)
        {
            if (CouldBallCatchUp(otherBall, radiusSum, collisionT1)) return collisionT1;
            return -1;

        }
        // if somehow it is not valid (unsure if possible), use the other
        //else if (collisionT2 >= 0.00001 && collisionT2 <= 1)
        //{
        //    if (CouldBallCatchUp(otherBall, radiusSum, collisionT2)) return collisionT2;
        //    return -1;
        //}
        // both outside of the trajectory segment, so no collision
        return -1;
    }

    private bool CouldBallCatchUp(IBall otherBall, double radiusSum, double collisionT)
    {
        IVector collisionPoint = ProgressTToPoint(collisionT);

        // normalized from travelling ball towards stationary ball
        Vector normal = new Vector()
        {
            X = (otherBall.X - collisionPoint.X) / radiusSum,
            Y = (otherBall.Y - collisionPoint.Y) / radiusSum
        };


        double travellingBallVelN = ball.Velocity.Dot(normal);
        double otherBallVelN = otherBall.Velocity.Dot(normal);

        // travelling ball is slower than the other ball in the direction of the normal,
        // and they are not moving towards each other, so the travelling ball cannot catch up to the other ball and collide with it.
        if (Math.Abs(travellingBallVelN) < Math.Abs(otherBallVelN) && travellingBallVelN * otherBallVelN >= 0)
        {
            return false;
        }
        return true;
    }

    public IVector ProgressTToPoint(double t)
    {
        return new Vector()
        {
            X = StartingX + t * DiffX,
            Y = StartingY + t * DiffY
        };
    } 
}
