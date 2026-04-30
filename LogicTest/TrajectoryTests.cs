using Data;
using Logic;

namespace LogicTest;

[TestClass]
public sealed class TrajectoryTests
{
    [TestMethod]
    public void TrajectoryInitTest()
    {
        Trajectory trajectory = new Trajectory(new FakeBall(0.0, 0.0, 1.0, 1.0), 1.0);


        Assert.AreEqual(0.0, trajectory.StartingX, 0.0001);
        Assert.AreEqual(0.0, trajectory.StartingY, 0.0001);
        Assert.AreEqual(1.0, trajectory.EndingX, 0.0001);
        Assert.AreEqual(1.0, trajectory.EndingY, 0.0001);
        Assert.AreEqual(1.0, trajectory.DiffX, 0.0001);
        Assert.AreEqual(1.0, trajectory.DiffY, 0.0001);


    }

    [TestMethod]
    public void TrajectoryCollideWithHalfPlaneTest()
    {
        Trajectory trajectory = new Trajectory(new FakeBall(0.0, 1.0, 1.0, 1.0), 1.0);
        
        Assert.AreEqual(-1, trajectory.CollideWithHalfPlane(1, -1, 0), "There shouldn't be any collision");

        Assert.AreEqual(0.5, trajectory.CollideWithHalfPlane(1, 1, -2));


    }

    [TestMethod]
    public void TrajectoryCollideWithBallTest()
    {
        Trajectory trajectory = new Trajectory(new FakeBall(0.0, 1.0, 1.0, 1.0), 1.0);


        Assert.AreEqual(-1, trajectory.CollideWithBall(new FakeBall(1.0, 1.0, 0.0, 0.0)), "Other ball is too far to collide");

        Assert.AreEqual(0.5 - (0.03 + 0.03) / Math.Sqrt(2), trajectory.CollideWithBall(new FakeBall(0.5, 1.5, 0, 0)), 0.00001, "Other ball is right on the collision trajectory, should collide");

    }


}