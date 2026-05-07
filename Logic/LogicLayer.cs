using System.Collections.ObjectModel;
using System.Diagnostics;
using Data;

namespace Logic
{
    public class LogicLayer : ILogic
    {

        IData layerUnderneathAPI;

        private ObservableCollection<LogicBall> balls { get; }
        private List<Task> _tasks = new();

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


        public void Start(int ballCount, Action<IBall> upperLayerHandler)
        {
            balls.Clear();
            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();
            _tasks.Clear();

            if (ballCount < 0)
            {
                throw new ArgumentException("Can't initialize a simulation with a negative number of balls.");
            }

            Action<IDataBall> registerBallWithUpperLayerHandler = (ball) =>
            {
                LogicBall logicBall = new LogicBall(ball);
                balls.Add(logicBall);
                upperLayerHandler(logicBall);

                _tasks.Add(Task.Run(() => logicBall.RunSimulationLoopAsync(balls, tokenSource.Token)));
            };

            layerUnderneathAPI.Start(ballCount, registerBallWithUpperLayerHandler);
        }


        /// <summary>
        /// If a main loop is currently running, signals it to stop by canceling the associated CancellationTokenSource.
        /// </summary>
        public void AbandonMainLoop()
        {
            tokenSource?.Cancel();
        }
    }
}