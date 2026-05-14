using Data;

namespace Logic
{
    public interface ILogic
    {
        public static readonly double FPS = 60;

        Task Start(int ballCount, Action<IBall> upperLayerHandler);

        void AbandonMainLoop();

    }
}