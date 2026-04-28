using System.ComponentModel;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {

        IVector Velocity { get; }

        double X { get; }
        double Y { get; }

        double Radius { get; }

        double Weight { get; }
    }
}
