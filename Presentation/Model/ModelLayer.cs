using System.Collections.ObjectModel;
using Logic;

namespace Model
{
    public class ModelLayer : ModelAbstractAPI
    {
        private readonly LogicAbstractAPI _logicLayer;
        private Task? mainLoopTask;
        public override ObservableCollection<BallModel> Balls { get; } = new ObservableCollection<BallModel>();

        public ModelLayer()
        {
            _logicLayer = LogicAbstractAPI.GetLogicLayer();
        }

        public ModelLayer(LogicAbstractAPI logicLayer)
        {
            _logicLayer = logicLayer;
        }

        public override async Task StartSimulation(int ballCount)
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

        public override void Dispose()
        {
            _logicLayer.Dispose();
        }
    }
}