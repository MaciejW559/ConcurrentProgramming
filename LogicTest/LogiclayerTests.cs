using Logic;
using Data;

namespace LogicTest
{
    [TestClass]
    public class LogicLayerTests
    {
        private class FakeDataApi : IData
        {
            public int StartedBallsCount { get; private set; } = 0;

            public void Start(int ballCount, Action<IDataBall> upperLayerHandler)
            {
                StartedBallsCount = ballCount;
                for (int i = 0; i < ballCount; i++)
                {
                    upperLayerHandler(new DataBall());
                }
            }
        }

        [TestMethod]
        public void Start_ShouldInitializeBallsAndInvokeHandler()
        {
            var fakeData = new FakeDataApi();
            var logicLayer = new LogicLayer(fakeData);
            int receivedBalls = 0;

            _ = logicLayer.Start(5, (ball) => { receivedBalls++; });

            Assert.AreEqual(5, fakeData.StartedBallsCount, "Warstwa logiki powinna przekazać odpowiednią liczbę do warstwy danych.");
            Assert.AreEqual(5, receivedBalls, "Warstwa logiki powinna powiadomić wyższą warstwę (wywołać handler) dla każdej utworzonej kuli.");
        }

        [TestMethod]
        public async Task AbandonMainLoop_ShouldNotThrowException()
        {
            var fakeData = new FakeDataApi();
            var logicLayer = new LogicLayer(fakeData);

            Task loopCreationTask = logicLayer.Start(1, (ball) => { });
            logicLayer.AbandonMainLoop();
            await loopCreationTask;
            Assert.IsTrue(true);
        }

    }
}
