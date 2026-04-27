using System.ComponentModel;

namespace Data
{
    public interface IBall : INotifyPropertyChanged
    {

        IVector Velocity { get; }

        double X { get; }
        double Y { get; }

        double RADIUS { get; }

        double WEIGHT { get; }
    }
}
