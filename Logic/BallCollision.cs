using Data;

namespace Logic;

internal class BallCollision : ICollision
{
    private readonly IDataBall travellingBall;
    private readonly IDataBall stationaryBall;
    /// <copydoc cref="ICollision.TPosition"/>
    public double TPosition { get; private set; }
    public Trajectory Trajectory { get; }


    public BallCollision(IDataBall travellingBall, IDataBall stationaryBall, Trajectory trajectory)
    {
        TPosition = trajectory.CollideWithBall(stationaryBall.X, stationaryBall.Y, stationaryBall.Radius);
        this.travellingBall = travellingBall;
        this.stationaryBall = stationaryBall;
        Trajectory = trajectory;
    }


    /// <copydoc cref="ICollision.PerformCollision"/>
    public void PerformCollision()
    {
        IVector newPos = Trajectory.ProgressTToPoint(TPosition);

        double radiusSum2 = (travellingBall.Radius + stationaryBall.Radius) * (travellingBall.Radius + stationaryBall.Radius);
        double weightSum = travellingBall.Weight + stationaryBall.Weight;

        // normalized from travelling ball towards stationary ball
        Vector normal = new Vector()
        {
            X = (stationaryBall.X - newPos.X) / radiusSum2,
            Y = (stationaryBall.Y - newPos.Y) / radiusSum2
        };


        double v1N = travellingBall.Velocity.Dot(normal);
        double v2N = stationaryBall.Velocity.Dot(normal);

        double newV1N = (v1N * (travellingBall.Weight - stationaryBall.Weight) + 2 * stationaryBall.Weight * v2N) / weightSum;
        double newV2N = (v2N * (stationaryBall.Weight - travellingBall.Weight) + 2 * travellingBall.Weight * v1N) / weightSum;

        IVector newVel1 = new Vector
        {
            X = travellingBall.Velocity.X + (newV1N - v1N) * normal.X,
            Y = travellingBall.Velocity.Y + (newV1N - v1N) * normal.Y
        };
        IVector newVel2 = new Vector
        {
            X = stationaryBall.Velocity.X + (newV2N - v2N) * normal.X,
            Y = stationaryBall.Velocity.Y + (newV2N - v2N) * normal.Y
        };

        travellingBall.Update(newPos, newVel1);
        stationaryBall.Update(null, newVel2);
    }
}
