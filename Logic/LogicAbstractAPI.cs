using Data;

namespace Logic
{
    public abstract class LogicAbstractAPI : IDisposable
    {
        protected static readonly double FPS = 60;

        private static Lazy<LogicAbstractAPI> modelInstance = new Lazy<LogicAbstractAPI>(() => new LogicLayerImplementation());
        public static LogicAbstractAPI GetLogicLayer()
        {
            return modelInstance.Value;
        }
        public abstract void Dispose();

        #region public API

        public abstract void Start(int ballCount, Action<IBall> upperLayerHandler);

        /// <summary>
        /// Move all balls in the simulation by the given delta time.
        /// This method should be called periodically to update the positions of the balls based on their velocities.
        /// </summary>
        /// <param name="deltaTime"> elapsed time in seconds </param>
        public abstract void Move(double deltaTime);

        public abstract Task SequentialMainLoop();

        public abstract void AbandonMainLoop();

        #endregion public API
    }
}
