namespace Data
{
    public interface IData : IDisposable
    {
        static readonly double SIMULATION_ROOM_ASPECT_RATIO = 4.0 / 3.0;

        void Start(int ballCount, Action<IDataBall> upperLayerHandler);

    }
}
