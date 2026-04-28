using Data;

namespace Logic;

/// <summary>
/// Class not aware of the mess with [0, 1] and [0, aspectRatio]
/// Assumes the ball is a circle, therefore ALL values should come prescaled to [0, aspectRatio] on X
/// </summary>
internal class Trajectory
{
    public double StartingX { get; set; }
    public double StartingY { get; set; }

    public double EndingX { get; set; }
    public double EndingY { get; set; }

    public double DiffX { get; set; }
    public double DiffY { get; set; }

    public double Radious { get; set; }

    public Trajectory(double startingX, double startingY, double diffX, double diffY, double radious)
    {
        StartingX = startingX;
        StartingY = startingY;
        DiffX = diffX;
        DiffY = diffY;
        EndingX = StartingX + DiffX;
        EndingY = StartingY + DiffY;
        Radious = radious;
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
        if (a * StartingX + b * StartingY + c > 0)
        {
            throw new ArgumentException("The trajectory starts in the half-plane, which is an unintended edge case");
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

 

    public double CollideWithBall(double otherBallX, double otherBallY, double ballRadious)
    {
        // The trajectory is a straight line, so we can find the distance from the center of the ball to the line,
        // and if it is less than the sum of the ball's radious and the trajectory's radious, then they could collide.
        double a = DiffY;
        double b = -DiffX;
        double c = EndingX * StartingY - StartingX * EndingY;

        double radiusSum = Radious + ballRadious;

        double distance = Math.Abs(a * otherBallX + b * otherBallY + c) / Math.Sqrt(a * a + b * b);
        if (distance > radiusSum)
        {
            // The trajectory does not collide with the ball at all.
            return -1;
        }


        // closest point on the straight extending from the trajectory to the ball
        double t = (-b * (otherBallX - StartingX)  + a * (otherBallY - StartingY)) / (a * a + b * b);

        double dt = Math.Sqrt(radiusSum * radiusSum - distance * distance);

        double collisionT1 = t - dt;
        double collisionT2 = t + dt;

        // if the collision point closer to the starting ball position if valid, return it
        if (collisionT1 >= 0 && collisionT1 <= 1)
        {
            return collisionT1;
        }
        // if somehow it is not valid (unsure if possible), use the other
        else if (collisionT2 >= 0 && collisionT2 <= 1)
        {
            return collisionT2;
        }
        // both outside of the trajectory segment, so no collision
        return -1;
    }

    public IVector ProgressTToPoint(double t)
    {
        return new Vector()
        {
            X = StartingX + t * (EndingX - StartingX),
            Y = StartingY + t * (EndingY - StartingY)
        };
    } 
}
