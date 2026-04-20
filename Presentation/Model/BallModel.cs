using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data; // Zależność do interfejsów IBall / IVector (które przekazuje Logika)

namespace Model
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;


        public double Diameter { get; }

        public double X => (_ball.X * ModelAbstractAPI.DEFAULT_WIDTH) - (Diameter / 2.0);
        public double Y => (_ball.Y * ModelAbstractAPI.DEFAULT_HEIGHT) - (Diameter / 2.0);

        public BallModel(IBall ball)
        {
            _ball = ball;
            // Assuming the aspect ratio is maintained, we can use RADIUS_X for both dimensions.
            Diameter = ball.RADIUS_Y * 2.0 * ModelAbstractAPI.DEFAULT_HEIGHT;

            _ball.NewPositionNotification += OnPositionChanged;
        }

        private void OnPositionChanged(object? sender, IVector vector)
        {
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}