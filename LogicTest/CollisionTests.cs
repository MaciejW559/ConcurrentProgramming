using Data;
using Logic;

namespace LogicTest;

[TestClass]
public sealed class CollisionTests
{
    private readonly double _inverseAspectRatio = 1.0 / IData.SIMULATION_ROOM_ASPECT_RATIO;
    [TestMethod]
    public void SingleBallCollisionTest()
    {
        var data_ball1 = IDataBall.FromNormalizedCoords(0.3, 0.6, new Vector { X = 0.2, Y = -0.1 });
        var logic_ball1 = new LogicBall(data_ball1);

        var data_ball2 = IDataBall.FromNormalizedCoords(0.4, 0.6, new Vector { X = -0.2, Y = -0.1 });
        var logic_ball2 = new LogicBall(data_ball2);


        logic_ball1.Move(1.0, [logic_ball2]);
        // first ball should have moved, collided with the 2nd ball, changed velocity, and then moved the rest of the way with the new velocity
        Assert.AreEqual(0.2951, logic_ball1.X, 0.0001);
        Assert.AreEqual(0.3376, logic_ball1.Y, 0.0001);
        Assert.AreEqual(-0.0955, logic_ball1.Velocity.X, 0.0001);
        Assert.AreEqual(-0.3342, logic_ball1.Velocity.Y, 0.0001);

        // 2nd ball should have been hit by the first ball, changed velocity, but not moved itself
        Assert.AreEqual(0.4, logic_ball2.X, 0.0001);
        Assert.AreEqual(0.6, logic_ball2.Y, 0.0001);
        Assert.AreEqual(0.0955, logic_ball2.Velocity.X, 0.0001);
        Assert.AreEqual(0.1342, logic_ball2.Velocity.Y, 0.0001);

        logic_ball2.Move(1.0, [logic_ball1]);

        // only after calling move on the 2nd ball should it actually move, and it should move with the new velocity it got from the collision
        Assert.AreEqual(0.4955, logic_ball2.X, 0.0001);
        Assert.AreEqual(0.7342, logic_ball2.Y, 0.0001);

    }


}