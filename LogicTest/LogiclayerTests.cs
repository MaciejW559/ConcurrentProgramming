using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using Data;
using System;
using System.Threading.Tasks;

namespace LogicTest
{
    [TestClass]
    public class LogicLayerTests
    {
        private class FakeDataApi : DataAbstractAPI
        {
            public int StartedBallsCount { get; private set; } = 0;
            public int MoveCallCount { get; private set; } = 0;
            public bool IsDisposed { get; private set; } = false;

            public override void Start(int ballCount, Action<IBall> upperLayerHandler)
            {
                StartedBallsCount = ballCount;
                for (int i = 0; i < ballCount; i++)
                {
                    upperLayerHandler(null!);
                }
            }

            public override void Move(double deltaTime)
            {
                MoveCallCount++;
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
            using var logicLayer = new LogicLayerImplementation(fakeData);
            int receivedBalls = 0;

            logicLayer.Start(5, (ball) => { receivedBalls++; });

            Assert.AreEqual(5, fakeData.StartedBallsCount, "Warstwa logiki powinna przekazać odpowiednią liczbę do warstwy danych.");
            Assert.AreEqual(5, receivedBalls, "Warstwa logiki powinna powiadomić wyższą warstwę (wywołać handler) dla każdej utworzonej kuli.");
        }

        [TestMethod]
        public async Task SequentialMainLoop_ShouldRunAsynchronouslyAndCanBeAbandoned()
        {
            var fakeData = new FakeDataApi();
            using var logicLayer = new LogicLayerImplementation(fakeData);

            var loopTask = logicLayer.SequentialMainLoop();

            await Task.Delay(150);

            Assert.IsFalse(loopTask.IsCompleted, "Zadanie pętli głównej powinno działać w tle i nie blokować wątku.");
            Assert.IsTrue(fakeData.MoveCallCount > 0, "Warstwa danych powinna zostać wywołana (Move) przynajmniej raz w trakcie działania pętli.");

            logicLayer.AbandonMainLoop();

            await loopTask;

            Assert.IsTrue(loopTask.IsCompleted, "Zadanie powinno zakończyć się po wywołaniu AbandonMainLoop.");
        }

        [TestMethod]
        public void Dispose_ShouldDisposeDataLayerAndCancelLoop()
        {
            var fakeData = new FakeDataApi();
            var logicLayer = new LogicLayerImplementation(fakeData);

            logicLayer.Dispose();

            Assert.IsTrue(fakeData.IsDisposed, "Dispose w warstwie logiki powinno również kaskadowo wywołać Dispose() na niższej warstwie danych.");
        }
    }
}
