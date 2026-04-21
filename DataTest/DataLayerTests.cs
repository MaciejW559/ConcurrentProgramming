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

                Assert.HasCount(10, receivedBalls, "Nie wygenerowano oczekiwanej liczby 10 kul.");

                foreach (var ball in receivedBalls)
                {
                    Assert.IsTrue(ball.X >= 0 && ball.X <= 1.0, $"Kula znalazła się poza planszą dla X: {ball.X}");
                    Assert.IsTrue(ball.Y >= 0 && ball.Y <= 1.0, $"Kula znalazła się poza planszą dla Y: {ball.Y}");
                }
            }
        }

        
    }
}