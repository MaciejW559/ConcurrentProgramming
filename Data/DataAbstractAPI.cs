namespace Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        public static readonly double SIMULATION_ROOM_ASPECT_RATIO = 4.0 / 3.0;

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataLayerImplementation());
        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }
        public abstract void Dispose();

        #region public API

        public abstract void Start(int ballCount, Action<IDataBall> upperLayerHandler);

        #endregion public API
    }
}
