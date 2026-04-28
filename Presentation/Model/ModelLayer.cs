using System.Collections.ObjectModel;
using Logic;

namespace Model
{
    public class ModelLayer : IModel
    {
        private readonly ILogic _logicLayer;
        private Task? mainLoopTask;
        public ObservableCollection<BallModel> Balls { get; } = [];

        public ModelLayer(ILogic logicLayer)
        {
            _logicLayer = logicLayer;
        }


        public async Task StartSimulation(int ballCount)
        {
            Balls.Clear();
            _logicLayer.AbandonMainLoop();
            if (mainLoopTask != null)
            {
                await mainLoopTask;
            }

            _logicLayer.Start(ballCount, (newBall) =>
            {
                Balls.Add(new BallModel(newBall));
            });

            
            mainLoopTask = _logicLayer.SequentialMainLoop();
        }
    }
}