using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewModel;
using Model;
using System.Collections.ObjectModel;

namespace ViewModelTest
{
    [TestClass]
    public class MainViewModelTests
    {
        private class FakeModelApi : ModelAbstractAPI
        {
            public int StartedSimulationBallCount { get; private set; } = 0;
            public override ObservableCollection<BallModel> Balls { get; } = new ObservableCollection<BallModel>();

            public override void StartSimulation(int ballCount)
            {
                StartedSimulationBallCount = ballCount;
            }

            public override void Dispose() { }
        }

        [TestMethod]
        public void StartCommand_ShouldInvokeStartSimulationWithCorrectBallCount()
        {
            var fakeModel = new FakeModelApi();
            var viewModel = new MainViewModel(fakeModel);
            viewModel.BallCount = 8;

            viewModel.StartCommand.Execute(null);

            Assert.AreEqual(8, fakeModel.StartedSimulationBallCount, "ViewModel powinien przekazać wartość BallCount do Modelu.");
        }
    }
}
