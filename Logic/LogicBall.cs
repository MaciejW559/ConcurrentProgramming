using System.ComponentModel;
using System.Runtime.CompilerServices;
using Data;

namespace Logic
{
    internal class LogicBall : IBall
    {
        private static readonly double INVERSE_ASPECT_RATIO = 1.0 / DataAbstractAPI.SIMULATION_ROOM_ASPECT_RATIO;
        private readonly double _left;
        private readonly double _right;
        private readonly double _top;
        private readonly double _bottom;
        

        public event PropertyChangedEventHandler? PropertyChanged;

        public double X => _dataBall.X;
        public double Y => _dataBall.Y;
        public double RADIUS => _dataBall.RADIUS;
        public double WEIGHT => _dataBall.WEIGHT;
        public IVector Velocity => _dataBall.Velocity;


        private readonly IDataBall _dataBall;

        public LogicBall(IDataBall dataBall) { 
            _dataBall = dataBall;

            _left = RADIUS * INVERSE_ASPECT_RATIO;
            _right = 1 - _left;

            _top = RADIUS;
            _bottom = 1 - _top;

            if (!IsInBoundsX(dataBall.X)) throw new ArgumentException("Initial Databall position out of bounds");
            if (!IsInBoundsY(dataBall.Y)) throw new ArgumentException("Initial Databall position out of bounds");
            
            

            _dataBall.PropertyChanged += DataBall_PropertyChanged;
        }

        private void DataBall_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != null) return;

            if (!IsInBoundsX(X))
            {
                if (X < _left) _dataBall.MirrorAlongStraight(-1, 0, _left);
                else _dataBall.MirrorAlongStraight(-1, 0, _right);
                return;
            }

            if (!IsInBoundsY(Y))
            {
                if (Y < _top) _dataBall.MirrorAlongStraight(0, -1, _top);
                else _dataBall.MirrorAlongStraight(0, -1, _bottom);
                return;
            }

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
    }
}
