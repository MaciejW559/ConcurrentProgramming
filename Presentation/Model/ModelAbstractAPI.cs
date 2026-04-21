using System;
using System.Collections.ObjectModel;

namespace Model
{
    public abstract class ModelAbstractAPI : IDisposable
    {
        public static double DEFAULT_WIDTH = 560;
        public static double DEFAULT_HEIGHT = 420;

        public static ModelAbstractAPI CreateApi() => new ModelLayer();

        public abstract ObservableCollection<BallModel> Balls { get; }

        public abstract Task StartSimulation(int ballCount);

        public abstract void Dispose();
    }
}