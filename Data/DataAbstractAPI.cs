using Datagit;

namespace Data
{
    public abstract class DataAbstractAPI : IDisposable
    {

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataLayerImplementation());
        public static DataAbstractAPI GetDataLayer()
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

        #endregion public API
    }

    public interface IVector
    {
        double X { get; init; }
        double Y { get; init; }

        void FlipX();

        void FlipY();
    }

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;

        IVector Velocity { get; init; }

        double X { get; init; }
        double Y { get; init; }
    }
}
