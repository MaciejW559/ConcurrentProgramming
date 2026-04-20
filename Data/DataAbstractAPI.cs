using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataLayerImplementation(60));
        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }
        public abstract void Dispose();

        #region public API

        public abstract void Start(int ballCount, Action<IBall> upperLayerHandler);

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
