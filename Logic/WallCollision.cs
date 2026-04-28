using Data;

namespace Logic;
/// <summary>
/// Class not aware of the mess with [0, 1] and [0, aspectRatio]
/// Assumes the ball is a circle, therefore ALL values should come prescaled to [0, aspectRatio] on X
/// </summary>
internal class WallCollision : ICollision
{
    private readonly IDataBall ball;
    private readonly double a;
    private readonly double b;
    private readonly double c;

    /// <copydoc cref="ICollision.TPosition"/>
    public double TPosition { get; private set; }
    public Trajectory Trajectory { get; }

    /// <summary>
    /// Create Collision object with all the necessary detail to perform a collision with a wall defined by ax + by + c >= 0
    /// </summary>
    /// <param name="ball"> ball that will be colliding</param>
    /// <param name="trajectory"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public WallCollision(IDataBall ball, Trajectory trajectory, double a, double b, double c)
    {
        TPosition = trajectory.CollideWithHalfPlane(a, b, c);
        this.ball = ball;
        Trajectory = trajectory;
        this.a = a;
        this.b = b;
        this.c = c;
    }


    /// <copydoc cref="ICollision.PerformCollision"/>
    public void PerformCollision()
    {
        IVector newPos = Trajectory.ProgressTToPoint(TPosition);

        double c = -b * ball.Velocity.X + a * ball.Velocity.Y;
        double a2B2 = a * a + b * b;

        IVector newVel = new Vector {
            X = 2 * (-b * c) / a2B2 - ball.Velocity.X,
            Y = 2 * (a * c) / a2B2 - ball.Velocity.Y
        };
        
        ball.Update(newPos, newVel);
    }

}
