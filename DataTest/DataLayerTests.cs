using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;
using System.Collections.Generic;

namespace DataTest
{
    [TestClass]
    public class DataLayerTests
    {
        [TestMethod]
        public void Start_ShouldCreateCorrectNumberOfBalls()
        {
            using (DataAbstractAPI dataApi = DataAbstractAPI.GetDataLayer())
            {
                int receivedBallsCount = 0;

                dataApi.Start(12, (ball) =>
                {
                    receivedBallsCount++;
                });

                Assert.AreEqual(12, receivedBallsCount, "API danych wygenerowało nieprawidłową liczbę kul.");
            }
        }

        [TestMethod]
        public void Start_ShouldCreateBallsWithinBounds()
        {
            using (DataAbstractAPI dataApi = DataAbstractAPI.GetDataLayer())
            {
                List<IBall> receivedBalls = new List<IBall>();

                dataApi.Start(10, (ball) =>
                {
                    receivedBalls.Add(ball);
                });

                Assert.HasCount(10, receivedBalls, "Nie wygenerowano oczekiwanej liczby 100 kul.");

                foreach (var ball in receivedBalls)
                {
                    Assert.IsTrue(ball.X >= 0 && ball.X <= 1.0, $"Kula znalazła się poza planszą dla X: {ball.X}");
                    Assert.IsTrue(ball.Y >= 0 && ball.Y <= 1.0, $"Kula znalazła się poza planszą dla Y: {ball.Y}");
                }
            }
        }

        [TestMethod]
        public void Move_ShouldChangeBallsPositions()
        {
            using (DataAbstractAPI dataApi = DataAbstractAPI.GetDataLayer())
            {
                List<IBall> receivedBalls = new List<IBall>();

                dataApi.Start(5, (ball) =>
                {
                    receivedBalls.Add(ball);
                });

                Assert.HasCount(5, receivedBalls, "Lista otrzymanych kul powinna zawierać 5 elementów.");

                var initialPositions = new List<(double X, double Y)>();
                foreach (var ball in receivedBalls)
                {
                    initialPositions.Add((ball.X, ball.Y));

                    Assert.IsNotNull(ball.Velocity, "Wektor prędkości kuli nie powinien być nullem.");
                    Assert.IsTrue(Math.Abs(ball.Velocity.X) > 0 || Math.Abs(ball.Velocity.Y) > 0,
                        "Prędkość kuli wynosi 0, przez co kula nigdy się nie poruszy.");
                }

                dataApi.Move(10.0);

                bool anyMoved = false;
                for (int i = 0; i < receivedBalls.Count; i++)
                {
                    double diffX = Math.Abs(receivedBalls[i].X - initialPositions[i].X);
                    double diffY = Math.Abs(receivedBalls[i].Y - initialPositions[i].Y);

                    if (diffX > 0.0001 || diffY > 0.0001)
                    {
                        anyMoved = true;
                        break;
                    }
                }

                Assert.IsTrue(anyMoved, "Żadna z kul nie zmieniła swojej pozycji po wywołaniu metody Move. Wartości X i Y pozostały identyczne.");
            }
        }
    }
}