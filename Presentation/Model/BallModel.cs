using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data; // Zależność do interfejsów IBall / IVector (które przekazuje Logika)

namespace Model
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;

        private const double CanvasWidth = 560;
        private const double CanvasHeight = 420;

        public double Diameter => 0.06 * CanvasHeight;

        public double X => (_ball.X * CanvasWidth) - (Diameter / 2);
        public double Y => (_ball.Y * CanvasHeight) - (Diameter / 2);

        public BallModel(IBall ball)
        {
            _ball = ball;
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