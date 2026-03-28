using Data;
using LayerUnderneathAPI = Data.DataAbstractAPI;

namespace Logic
{
    internal class LogicLayerImplementation : LogicAbstractAPI
    {

        LayerUnderneathAPI layerUnderneathAPI;
        private bool disposed = false;

        public LogicLayerImplementation() : this(null) { }


        public LogicLayerImplementation(LayerUnderneathAPI? layerUnderneathAPI)
        {
            this.layerUnderneathAPI = layerUnderneathAPI == null ? LayerUnderneathAPI.GetDataLayer() : layerUnderneathAPI;
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

        public override void Start(int ballCount, Action<IBall> upperLayerHandler)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (ballCount < 0)
            {
                throw new ArgumentException("Can't initialize a simulation with a negative number of balls.");
            }
            layerUnderneathAPI.Start(ballCount, upperLayerHandler);
        }
    }
}
