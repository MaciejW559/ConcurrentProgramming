using System;
using System.Collections.ObjectModel;

namespace Model
{
    public interface IModel
    {
        static readonly double DEFAULT_WIDTH = 560;
        static readonly double DEFAULT_HEIGHT = 420;

        ObservableCollection<BallModel> Balls { get; }

        Task StartSimulation(int ballCount);
    }
}