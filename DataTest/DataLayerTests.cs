using Data;

namespace DataTest
{
    [TestClass]
    public class DataLayerTests
    {
        [TestMethod]
        public void Start_ShouldCreateCorrectNumberOfBalls()
        {
            IData dataApi = new DataLayer();
            int receivedBallsCount = 0;

            dataApi.Start(12, (ball) =>
            {
                receivedBallsCount++;
            });

            Assert.AreEqual(12, receivedBallsCount, "API danych wygenerowało nieprawidłową liczbę kul.");
        }

        [TestMethod]
        public void Start_ShouldCreateBallsWithinBounds()
        {
            IData dataApi = new DataLayer();
            List<IBall> receivedBalls = [];

            dataApi.Start(10, (ball) =>
            {
                receivedBalls.Add(ball);
            });

            Assert.HasCount(10, receivedBalls, "Nie wygenerowano oczekiwanej liczby 10 kul.");

            foreach (var ball in receivedBalls)
            {
                Assert.IsTrue(ball.X >= 0 && ball.X <= IData.SIMULATION_ROOM_ASPECT_RATIO, $"Kula znalazła się poza planszą dla X: {ball.X}");
                Assert.IsTrue(ball.Y >= 0 && ball.Y <= 1.0, $"Kula znalazła się poza planszą dla Y: {ball.Y}");
            }
        }

        
    }
}