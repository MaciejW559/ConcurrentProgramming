using Data;

namespace Logic
{
    public interface ILogic
    {
        protected static readonly double FPS = 60;


        void Start(int ballCount, Action<IBall> upperLayerHandler);

        /// <summary>
        /// Move all balls in the simulation by the given delta time.
        /// This method should be called periodically to update the positions of the balls based on their velocities.
        /// </summary>
        /// <param name="deltaTime"> elapsed time in seconds </param>
        void Move(double deltaTime);

        Task SequentialMainLoop();

        void AbandonMainLoop();

    }
}
