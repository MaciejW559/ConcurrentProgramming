using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data;

namespace Logic
{
    internal class LogicBall : IBall
    {
        private static readonly double INVERSE_ASPECT_RATIO = 1.0 / IData.SIMULATION_ROOM_ASPECT_RATIO;
        private readonly double _left;
        private readonly double _right;
        private readonly double _top;
        private readonly double _bottom;
        

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Intentionally separate from the underlying DataBall,
        /// to avoid race conditions which could happen if properties were read
        /// while the DataBall and LogicBall were in the middle of figuring out multiple bounces.
        /// </summary>
        public double X { get; private set; }
        public double Y { get; private set; }
        public IVector Velocity { get; private set; }

        public double Radius => _dataBall.Radius;
        public double Weight => _dataBall.Weight;


        private readonly IDataBall _dataBall;

        public LogicBall(IDataBall dataBall) { 
            _dataBall = dataBall;

            _left = Radius * INVERSE_ASPECT_RATIO;
            _right = 1 - _left;

            _top = Radius;
            _bottom = 1 - _top;

            // if it helps VS not recognizing the same damn line is in UpdatePropertiesFromDataBall and raising "fied not initialized" warning, then so be it
            Velocity = _dataBall.Velocity;
            UpdatePropertiesFromDataBall();

            if (!IsInBoundsX(dataBall.X)) throw new ArgumentException("Initial Databall position out of bounds");
            if (!IsInBoundsY(dataBall.Y)) throw new ArgumentException("Initial Databall position out of bounds");


            _dataBall.PropertyChanged += DataBall_PropertyChanged;
        }

        private void DataBall_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null) return;

            if (!IsInBoundsX(_dataBall.X))
            {
                if (_dataBall.X < _left) _dataBall.MirrorAlongStraight(-1, 0, _left);
                else _dataBall.MirrorAlongStraight(-1, 0, _right);
                return;
            }

            if (!IsInBoundsY(_dataBall.Y))
            {
                if (_dataBall.Y < _top) _dataBall.MirrorAlongStraight(0, -1, _top);
                else _dataBall.MirrorAlongStraight(0, -1, _bottom);
                return;
            }

            UpdatePropertiesFromDataBall();
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        /// <summary>
        /// Move the ball according to its velocity and the time elapsed since the last movement in seconds.
        /// Includes bouncing off the walls of the simulation room, which are located at x = 0, x = 1, y = 0 and y = 1.
        /// (ball bounds are tighter due to the non-zero radious of balls)
        /// 
        /// Bouncing is implemented in the listener method DataBall_PropertyChanged,
        /// which listens to changes in the position of the underlying DataBall.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Move(double deltaTime)
        {
            _dataBall.Move(deltaTime);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private bool IsInBoundsX(double coordinate)
        {
            if (coordinate > _right) return false;
            if (coordinate < _left) return false;
            return true;
        }

        private bool IsInBoundsY(double coordinate)
        {
            if (coordinate > _bottom) return false;
            if (coordinate < _top) return false;
            return true;
        }

        private void UpdatePropertiesFromDataBall()
        {
            Velocity = _dataBall.Velocity;
            X = _dataBall.X;
            Y = _dataBall.Y;
        }
    }
}
