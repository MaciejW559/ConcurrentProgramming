using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic;

namespace Model
{
    public class ModelLayer : IModel
    {
        private readonly ILogic _logicLayer;
        public ObservableCollection<BallModel> Balls { get; } = [];


        public ModelLayer(ILogic logicLayer)
        {
            _logicLayer = logicLayer;
        }


        public Task StartSimulation(int ballCount)
        {
            Balls.Clear();
            _logicLayer.AbandonMainLoop();

            _logicLayer.Start(ballCount, (newBall) =>
            {
                Balls.Add(new BallModel(newBall));
            });

            return Task.CompletedTask;
        }
    }
}