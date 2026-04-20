using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data; // Zależność do interfejsów IBall / IVector (które przekazuje Logika)

namespace Model
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;


        public double Diameter { get; }

        public double X { get; }
        public double Y { get; }

        public BallModel(IBall ball)
        {
            _ball = ball;
            // Assuming the aspect ratio is maintained, we can use RADIUS_X for both dimensions.
            Diameter = ball.RADIUS_X * 2 * ModelAbstractAPI.DEFAULT_WIDTH;
            X = _ball.X * ModelAbstractAPI.DEFAULT_WIDTH - Diameter / 2;
            Y = _ball.Y * ModelAbstractAPI.DEFAULT_HEIGHT - Diameter / 2;

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