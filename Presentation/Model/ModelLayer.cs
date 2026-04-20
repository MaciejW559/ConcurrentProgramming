using System.Collections.ObjectModel;
using Logic;

namespace Model
{
    internal class ModelLayer : ModelAbstractAPI
    {
        private readonly LogicAbstractAPI _logicLayer;
        public override ObservableCollection<BallModel> Balls { get; } = new ObservableCollection<BallModel>();

        public ModelLayer()
        {
            _logicLayer = LogicAbstractAPI.GetLogicLayer();
        }

        public override void StartSimulation(int ballCount)
        {
            Balls.Clear();
            _logicLayer.Start(ballCount, (newBall) =>
            {
                Balls.Add(new BallModel(newBall));
            });
        }

        public override void Dispose()
        {
            _logicLayer.Dispose();
        }
    }
}