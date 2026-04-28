using Data;

namespace Logic
{
    public abstract class LogicAbstractAPI : IDisposable
    {
        protected static readonly double FPS = 60;

        
        public abstract void Dispose();


        public abstract void Start(int ballCount, Action<IBall> upperLayerHandler);

        /// <summary>
        /// Move all balls in the simulation by the given delta time.
        /// This method should be called periodically to update the positions of the balls based on their velocities.
        /// </summary>
        /// <param name="deltaTime"> elapsed time in seconds </param>
        public abstract void Move(double deltaTime);

        public abstract Task SequentialMainLoop();

        public abstract void AbandonMainLoop();

    }
}
