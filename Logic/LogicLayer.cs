using System.Collections.ObjectModel;
using System.Diagnostics;
using Data;

namespace Logic
{
    public class LogicLayer : ILogic
    {

        IData layerUnderneathAPI;

        private ObservableCollection<LogicBall> balls { get; }

        /// <summary>
        /// CancellationTokenSource used to signal the main loop to stop.
        /// Called when AbandonMainLoop is called
        /// </summary>
        private CancellationTokenSource? tokenSource;



        public LogicLayer(IData layerUnderneathAPI)
        {
            this.layerUnderneathAPI = layerUnderneathAPI;
            balls = new ObservableCollection<LogicBall>();

        }


        public  void Start(int ballCount, Action<IBall> upperLayerHandler)
        {
            balls.Clear();
            if (ballCount < 0)
            {
                throw new ArgumentException("Can't initialize a simulation with a negative number of balls.");
            }

            Action<IDataBall> registerBallWithUpperLayerHandler = (ball) =>
            {
                LogicBall logicBall = new LogicBall(ball);
                balls.Add(logicBall);
                upperLayerHandler(logicBall);
            };
            
            layerUnderneathAPI.Start(ballCount, registerBallWithUpperLayerHandler);
        }

        /// <summary>
        /// Executes the application's main loop, updating the underlying layer at a fixed frame rate.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation of the main loop.</returns>
        public  async Task SequentialMainLoop()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1.0 / ILogic.FPS));
            var timestamp = Stopwatch.GetTimestamp();

            try
            {
                while (await timer.WaitForNextTickAsync(token))
                {
                    var elapsed = Stopwatch.GetElapsedTime(timestamp);
                    timestamp = Stopwatch.GetTimestamp();

                    Move(elapsed.TotalSeconds);
                }
            }
            catch (OperationCanceledException)
            {
                // expected, token triggered cancellation
            }
        }

        public  void Move(double deltaTime)
        {
            foreach (var ball in balls)
            {
                ball.Move(deltaTime, balls);
            }
        }


        /// <summary>
        /// If a main loop is currently running, signals it to stop by canceling the associated CancellationTokenSource.
        /// </summary>
        public  void AbandonMainLoop()
        {
            tokenSource?.Cancel();
        }
    }
}
