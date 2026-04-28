using Data;
using Logic;
using Model;
using System.ComponentModel;

namespace ModelTest
{
    [TestClass]
    public class ModelLayerTests
    {
        private class FakeBall : IBall
        {
            public IVector Velocity { get; init; } = null!;

            public double X { get; init; } = 10.0;
            public double Y { get; init; } = 10.0;

            public double RADIUS { get; } = 10.0;
            public double WEIGHT { get; } = 10.0;

            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private class FakeLogicApi : LogicAbstractAPI
        {
            public int StartedBallCount { get; private set; } = 0;

            public override void Start(int ballCount, Action<IBall> upperLayerHandler)
            {
                StartedBallCount = ballCount;
                for (int i = 0; i < ballCount; i++)
                {
                    upperLayerHandler(new FakeBall());
                }
            }

            public override Task SequentialMainLoop() => Task.CompletedTask;

            public override void Move(double _deltaTime) { }
            public override void AbandonMainLoop() { }
            public override void Dispose() { }
        }

        [TestMethod]
        public void StartSimulation_ShouldPopulateBallsCollection()
        {
            var fakeLogic = new FakeLogicApi();
            using var modelLayer = new ModelLayer(fakeLogic);

            _ = modelLayer.StartSimulation(3);

            Assert.AreEqual(3, fakeLogic.StartedBallCount, "Model powinien przekazać poprawną liczbę kul do logiki.");
            Assert.HasCount(3, modelLayer.Balls, "Model powinien utworzyć BallModel dla każdej otrzymanej kuli.");
        }
    }
}