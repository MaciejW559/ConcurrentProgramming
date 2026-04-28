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
            public bool IsDisposed { get; private set; } = false;

            public override void Start(int ballCount, Action<IDataBall> upperLayerHandler)
            {
                StartedBallsCount = ballCount;
                for (int i = 0; i < ballCount; i++)
                {
                    upperLayerHandler(new DataBall());
                }
            }


            public override void Dispose()
            {
                IsDisposed = true;
            }
        }

        [TestMethod]
        public void Start_ShouldInitializeBallsAndInvokeHandler()
        {
            var fakeData = new FakeDataApi();
            using var logicLayer = new LogicLayer(fakeData);
            int receivedBalls = 0;

            logicLayer.Start(5, (ball) => { receivedBalls++; });

            Assert.AreEqual(5, fakeData.StartedBallsCount, "Warstwa logiki powinna przekazać odpowiednią liczbę do warstwy danych.");
            Assert.AreEqual(5, receivedBalls, "Warstwa logiki powinna powiadomić wyższą warstwę (wywołać handler) dla każdej utworzonej kuli.");
        }

        [TestMethod]
        public async Task SequentialMainLoop_ShouldRunAsynchronouslyAndCanBeAbandoned()
        {
            var fakeData = new FakeDataApi();
            using var logicLayer = new LogicLayer(fakeData);

            IBall singularBall = null!;
            logicLayer.Start(1, (ball) => { singularBall = ball; });
            int moves = 0;

            Assert.IsNotNull(singularBall, "Handler powinien zostać wywołany i przekazać utworzoną kulę.");
            singularBall.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(IBall.X) || e.PropertyName == nameof(IBall.Y)) moves++; };

            var loopTask = logicLayer.SequentialMainLoop();

            await Task.Delay(150);

            Assert.IsFalse(loopTask.IsCompleted, "Zadanie pętli głównej powinno działać w tle i nie blokować wątku.");
            Assert.IsGreaterThan(0, moves, "Kulka powinna wysłać powiadomienie o swoim przemieszczeniu przynajmniej raz w trakcie działania pętli.");

            logicLayer.AbandonMainLoop();

            await loopTask;

            Assert.IsTrue(loopTask.IsCompleted, "Zadanie powinno zakończyć się po wywołaniu AbandonMainLoop.");
        }

        [TestMethod]
        public void Dispose_ShouldDisposeDataLayerAndCancelLoop()
        {
            var fakeData = new FakeDataApi();
            var logicLayer = new LogicLayer(fakeData);

            logicLayer.Dispose();

            Assert.IsTrue(fakeData.IsDisposed, "Dispose w warstwie logiki powinno również kaskadowo wywołać Dispose() na niższej warstwie danych.");
        }
    }
}
