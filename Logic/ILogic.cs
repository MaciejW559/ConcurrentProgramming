using Data;

namespace Logic
{
    public interface ILogic
    {
        public static readonly double FPS = 60;

        void Start(int ballCount, Action<IBall> upperLayerHandler);

        void AbandonMainLoop();

    }
}