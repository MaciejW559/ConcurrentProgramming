using Data;
using Logic;

namespace LogicTest
{
    [TestClass]
    public sealed class LogicBallTests
    {
        [TestMethod]
        public void Ball_Initialization_Test()
        {
            var data_ball = new DataBall
            {
                X = 0.3,
                Y = 0.6,
                Velocity = new Vector { X = 0.05, Y = -0.05 }
            };

            var logic_ball = new LogicBall(data_ball);

            int calls = 0;
            logic_ball.PropertyChanged += (sender, e) =>
            {
                calls++;
            };


            Assert.AreEqual(0.3, logic_ball.X, 0.0001, "the initial value of X should be different");
            Assert.AreEqual(0.6, logic_ball.Y, 0.0001, "the initial value of Y should be different");

            data_ball.Move(1.0);

            Assert.AreEqual(0.35, logic_ball.X, 0.0001);
            Assert.AreEqual(0.55, logic_ball.Y, 0.0001);
            Assert.AreEqual(2, calls, "PropertyChanged event should have been raised twice (once for X and once for Y)");

        }

        private readonly double _inverseAspectRatio = 1.0 / DataAbstractAPI.SIMULATION_ROOM_ASPECT_RATIO;

        [TestMethod]
        public void Ball_Bounce_Left_Wall_Test()
        {
            var ball = new DataBall { X = 0.05, Y = 0.5, Velocity = new Data.Vector { X = -0.1, Y = 0.0 } };
            var logicBall = new LogicBall(ball);

            double leftWall = ball.RADIUS * _inverseAspectRatio;
            double targetX = ball.X + ball.Velocity.X; // 0.05 - 0.1 = -0.05
            double expectedX = leftWall + (leftWall - targetX);

            int calls = 0;
            logicBall.PropertyChanged += (s, e) => calls++;

            logicBall.Move(1.0);

            Assert.AreEqual(expectedX, logicBall.X, 0.0001, "Incorrect bounce off left wall");
            Assert.AreEqual(0.1, logicBall.Velocity.X, 0.0001, "Velocity.X has not been flipped");
            Assert.AreEqual(2, calls);
        }

        [TestMethod]
        public void Ball_Bounce_Right_Wall_Test()
        {
            var ball = new DataBall { X = 0.95, Y = 0.5, Velocity = new Data.Vector { X = 0.1, Y = 0.0 } };
            var logicBall = new LogicBall(ball);

            double rightWall = 1.0 - ball.RADIUS * _inverseAspectRatio;
            double targetX = ball.X + ball.Velocity.X; // 0.95 + 0.1 = 1.05
            double expectedX = rightWall + (rightWall - targetX);

            int calls = 0;
            logicBall.PropertyChanged += (s, e) => calls++;

            logicBall.Move(1.0);

            Assert.AreEqual(expectedX, logicBall.X, 0.0001, "Incorrect bounce off right wall");
            Assert.AreEqual(-0.1, logicBall.Velocity.X, 0.0001, "Velocity.X has not been flipped");
            Assert.AreEqual(2, calls);
        }

        [TestMethod]
        public void Ball_Bounce_Top_Wall_Test()
        {
            var ball = new DataBall { X = 0.5, Y = 0.05, Velocity = new Data.Vector { X = 0.0, Y = -0.1 } };
            var logicBall = new LogicBall(ball);

            double topWall = ball.RADIUS;
            double targetY = ball.Y + ball.Velocity.Y; // 0.05 - 0.1 = -0.05
            double expectedY = topWall + (topWall - targetY);

            int calls = 0;
            logicBall.PropertyChanged += (s, e) => calls++;

            logicBall.Move(1.0);

            Assert.AreEqual(expectedY, logicBall.Y, 0.0001, "Odbicie od górnej ściany (Y) jest nieprawidłowe.");
            Assert.AreEqual(0.1, logicBall.Velocity.Y, 0.0001, "Prędkość Y powinna zostać odwrócona.");
            Assert.AreEqual(2, calls);
        }

        [TestMethod]
        public void Ball_Bounce_Bottom_Wall_Test()
        {
            var ball = new DataBall { X = 0.5, Y = 0.95, Velocity = new Data.Vector { X = 0.0, Y = 0.1 } };
            var logicBall = new LogicBall(ball);

            double bottomWall = 1.0 - ball.RADIUS;
            double targetY = ball.Y + ball.Velocity.Y; // 0.95 + 0.1 = 1.05
            double expectedY = bottomWall - (targetY - bottomWall);

            int calls = 0;
            logicBall.PropertyChanged += (s, e) => calls++;

            logicBall.Move(1.0);

            Assert.AreEqual(expectedY, logicBall.Y, 0.0001, "Odbicie od dolnej ściany (Y) jest nieprawidłowe.");
            Assert.AreEqual(-0.1, logicBall.Velocity.Y, 0.0001, "Prędkość Y powinna zostać odwrócona.");
            Assert.AreEqual(2, calls);
        }

    }
}