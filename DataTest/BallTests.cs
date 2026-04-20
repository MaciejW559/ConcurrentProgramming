using Data;

namespace DataTest
{
    [TestClass]
    public sealed class BallTests
    {
        [TestMethod]
        public void Ball_Initialization_Test()
        {
            var ball = new Ball
            {
                X = 0.3,
                Y = 0.6,
                Velocity = new Vector { X = 0.05, Y = -0.05 }
            };

            Assert.AreEqual(0.3, ball.X, 0.0001, "the initial value of X should be different");
            Assert.AreEqual(0.6, ball.Y, 0.0001, "the initial value of Y should be different");
        }

        [TestMethod]
        public void Ball_Move_Test()
        {
            var ball = new Ball
            {
                X = 0.3,
                Y = 0.6,
                Velocity = new Vector { X = 0.05, Y = -0.05 }
            };

            ball.Move(1.0);

            Assert.AreEqual(0.35, ball.X, 0.0001, "the x value after the move does not change or it was done incorrectly");
            Assert.AreEqual(0.55, ball.Y, 0.0001, "the y value after the move does not change or it was done incorrectly");
        }

        [TestMethod]
        public void Ball_Is_Bounce_Test()
        {
            var ball = new Ball
            {
                X = 0.90,
                Y = 0.5,
                Velocity = new Data.Vector { X = 0.15, Y = 0.0 }
            };

            ball.Move(1.0);

            Assert.AreEqual(0.905, ball.X, 0.0001, "The X position is incorrect after bouncing off the right wall.");
            Assert.AreEqual(-0.15, ball.Velocity.X, 0.0001, "X-axis velocity did not reverse after hitting the wall.");
            Assert.AreEqual(0.5, ball.Y, 0.0001, "The Y position should remain unchanged.");
        }
    }
}