using Data;

namespace DataTest
{
    [TestClass]
    public sealed class BallTests
    {
        [TestMethod]
        public void Ball_Initialization_Test()
        {
            var ball = new DataBall(0.3, 0.6, new Vector { X = 0.05, Y = -0.05 });

            Assert.AreEqual(0.3, ball.X, 0.0001, "the initial value of X should be different");
            Assert.AreEqual(0.6, ball.Y, 0.0001, "the initial value of Y should be different");

            ball = IDataBall.FromNormalizedCoords(0.3, 0.6, new Vector { X = 0.05, Y = -0.05 });

            Assert.AreEqual(0.3 * IData.SIMULATION_ROOM_ASPECT_RATIO, ball.X, 0.0001, "the initial value of X should be different");
            Assert.AreEqual(0.6, ball.Y, 0.0001, "the initial value of Y should be different");
        }

        [TestMethod]
        public void Ball_Move_Test()
        {
            var ball = new DataBall(0.3, 0.6, new Vector { X = 0.05, Y = -0.05 });

            int calls = 0;
            ball.PropertyChanged += (sender, e) =>
            {
                calls++;
            };

            ball.Update(new Vector { X = 20, Y = 30 }, null); 

            Assert.AreEqual(20, ball.X, 0.0001, "the x value after the move does not change or it was done incorrectly");
            Assert.AreEqual(30, ball.Y, 0.0001, "the y value after the move does not change or it was done incorrectly");

            Assert.AreEqual(1, calls, "PropertyChanged event should have been raised");
        }
    }
}