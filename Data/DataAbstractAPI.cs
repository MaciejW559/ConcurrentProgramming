using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    internal class DataAbstractAPI
    {
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
    }
}
