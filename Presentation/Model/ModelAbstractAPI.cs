using System;
using System.Collections.ObjectModel;

namespace Model
{
    public abstract class ModelAbstractAPI : IDisposable
    {
        public static ModelAbstractAPI CreateApi() => new ModelLayer();

        public abstract ObservableCollection<BallModel> Balls { get; }

        public abstract void StartSimulation(int ballCount);

        public abstract void Dispose();
    }
}