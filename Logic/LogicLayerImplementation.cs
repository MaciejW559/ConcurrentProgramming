using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Data;
using LayerUnderneathAPI = Data.DataAbstractAPI;

namespace Logic
{
    internal class LogicLayerImplementation : LogicAbstractAPI
    {

        LayerUnderneathAPI layerUnderneathAPI;
        private bool disposed = false;

        private ObservableCollection<IBall> balls { get; }

        private PeriodicTimer timer;


        public LogicLayerImplementation() : this(null) { }


        public LogicLayerImplementation(LayerUnderneathAPI? layerUnderneathAPI)
        {
            this.layerUnderneathAPI = layerUnderneathAPI == null ? LayerUnderneathAPI.GetDataLayer() : layerUnderneathAPI;
            balls = new ObservableCollection<IBall>();
            timer = new PeriodicTimer(TimeSpan.FromSeconds(1.0 / FPS));
        }


        public override async void Start(int ballCount, Action<IBall> upperLayerHandler)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (ballCount < 0)
            {
                throw new ArgumentException("Can't initialize a simulation with a negative number of balls.");
            }

            Action<IBall> registerBallWithUpperLayerHandler = (ball) =>
            {
                balls.Add(ball);
                upperLayerHandler(ball);
            };
            
            layerUnderneathAPI.Start(ballCount, registerBallWithUpperLayerHandler);
        }

        public override async Task SequentialMainLoop()
        {
            var timestamp = Stopwatch.GetTimestamp();

            while (await timer.WaitForNextTickAsync())
            {
                var elapsed = Stopwatch.GetElapsedTime(timestamp);
                timestamp = Stopwatch.GetTimestamp();

                layerUnderneathAPI.Move(elapsed.TotalSeconds);
            }
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
                timer.Dispose();
                layerUnderneathAPI.Dispose();
            }
            disposed = true;
        }
    }
}
