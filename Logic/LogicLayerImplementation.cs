using System.Collections.ObjectModel;
using Data;
using LayerUnderneathAPI = Data.DataAbstractAPI;

namespace Logic
{
    internal class LogicLayerImplementation : LogicAbstractAPI
    {

        LayerUnderneathAPI layerUnderneathAPI;
        private bool disposed = false;

        private ObservableCollection<IBall> _balls { get; }
        public ReadOnlyObservableCollection<IBall> Balls { get; }

        public LogicLayerImplementation() : this(null) { }


        public LogicLayerImplementation(LayerUnderneathAPI? layerUnderneathAPI)
        {
            this.layerUnderneathAPI = layerUnderneathAPI == null ? LayerUnderneathAPI.GetDataLayer() : layerUnderneathAPI;
            _balls = new ObservableCollection<IBall>();
            Balls = new ReadOnlyObservableCollection<IBall>(_balls);

        }


        public override void Start(int ballCount, Action<IBall> upperLayerHandler)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (ballCount < 0)
            {
                throw new ArgumentException("Can't initialize a simulation with a negative number of balls.");
            }

            Action<IBall> registerBallWithUpperLayerHandler = (ball) =>
            {
                _balls.Add(ball);
                upperLayerHandler(ball);
            };
            
            layerUnderneathAPI.Start(ballCount, registerBallWithUpperLayerHandler);
        }



        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (disposing)
            {
                layerUnderneathAPI.Dispose();
            }
            disposed = true;
        }
    }
}
