using System.Collections.ObjectModel;
using System.Diagnostics;
using Data;

namespace Logic
{
    public class LogicLayer : ILogic
    {

        IData layerUnderneathAPI;

        private readonly ObservableCollection<LogicBall> balls = [];
        private readonly List<Task> _tasks = [];
        private Barrier? _barrier;

        /// <summary>
        /// CancellationTokenSource used to signal the main loop to stop.
        /// Called when AbandonMainLoop is called
        /// </summary>
        private CancellationTokenSource? tokenSource;


        public LogicLayer(IData layerUnderneathAPI)
        {
            this.layerUnderneathAPI = layerUnderneathAPI;
        }


        public async Task Start(int ballCount, Action<IBall> upperLayerHandler)
        {
            if (ballCount < 0)
            {
                throw new ArgumentException("Can't initialize a simulation with a negative number of balls.");
            }
            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();

            foreach (Task task in _tasks)
            {
                await task;
            }
            
            _tasks.Clear();
            balls.Clear();

            ThreadPool.SetMinThreads(ballCount + 2, ballCount + 2);
            _barrier = new Barrier(ballCount);

            Action<IDataBall> registerBallWithUpperLayerHandler = (ball) =>
            {
                LogicBall logicBall = new LogicBall(ball);
                balls.Add(logicBall);
                upperLayerHandler(logicBall);

                _tasks.Add(
                    Task.Run(() => logicBall.RunSimulationLoopAsync(balls, tokenSource.Token, _barrier))
                    );
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