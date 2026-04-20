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

        private CancellationTokenSource? tokenSource;


        public LogicLayerImplementation() : this(null) { }


        public LogicLayerImplementation(LayerUnderneathAPI? layerUnderneathAPI)
        {
            this.layerUnderneathAPI = layerUnderneathAPI == null ? LayerUnderneathAPI.GetDataLayer() : layerUnderneathAPI;
            balls = new ObservableCollection<IBall>();

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
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1.0 / FPS));
            var timestamp = Stopwatch.GetTimestamp();

            try
            {
                while (await timer.WaitForNextTickAsync(token))
                {
                    var elapsed = Stopwatch.GetElapsedTime(timestamp);
                    timestamp = Stopwatch.GetTimestamp();

                    layerUnderneathAPI.Move(elapsed.TotalSeconds);
                }
            }
            catch (OperationCanceledException)
            {
                // expected, token triggered cancellation
            }
        }

        public override void AbandonMainLoop()
        {
            tokenSource?.Cancel();
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
                //timer.Dispose();
                layerUnderneathAPI.Dispose();
            }
            disposed = true;
        }
    }
}
