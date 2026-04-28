using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data; // Zależność do interfejsów IBall / IVector (które przekazuje Logika)

namespace Model
{
    public class BallModel : INotifyPropertyChanged
    {
        private readonly IBall _ball;


        public double Diameter { get; }

        public double X => (_ball.X * IModel.DEFAULT_WIDTH) - (Diameter / 2.0);
        public double Y => (_ball.Y * IModel.DEFAULT_HEIGHT) - (Diameter / 2.0);

        public BallModel(IBall ball)
        {
            _ball = ball;
            Diameter = ball.Radius * 2.0 * IModel.DEFAULT_HEIGHT;

            _ball.PropertyChanged += OnPositionChanged;
        }

        private void OnPositionChanged(object? sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(IBall.X):
                    OnPropertyChanged(nameof(X));
                    break;
                case nameof(IBall.Y):
                    OnPropertyChanged(nameof(Y));
                    break;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}