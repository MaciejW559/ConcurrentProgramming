using System.Collections.ObjectModel;
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

        
        // Intentionally separate from the underlying DataBall,
        // to avoid race conditions which could happen if properties were read
        // while the DataBall and LogicBall were in the middle of figuring out multiple bounces.
        /// <summary>
        /// X coordinate normalized to [0, 1]
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// Y coordinate normalized to [0, 1]
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// Normalized velocity
        /// </summary>
        public IVector Velocity { get; private set; }

        public double Radius => _dataBall.Radius;
        public double Weight => _dataBall.Weight;


        private readonly IDataBall _dataBall;

        public LogicBall(IDataBall dataBall) { 
            _dataBall = dataBall;

            _left = Radius;
            _right = IData.SIMULATION_ROOM_ASPECT_RATIO - _left;

            _top = Radius;
            _bottom = 1 - _top;

            // if it helps VS not recognizing the same damn line is in UpdatePropertiesFromDataBall and raising "fied not initialized" warning, then so be it
            Velocity = _dataBall.Velocity;
            UpdatePropertiesFromDataBall();

            if (!IsInBoundsX(dataBall.X)) throw new ArgumentException("Initial Databall position out of bounds");
            if (!IsInBoundsY(dataBall.Y)) throw new ArgumentException("Initial Databall position out of bounds");
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
        public void Move(double deltaTime, Collection<LogicBall> balls)
        {
            while (true)
            {
                Trajectory trajectory = new Trajectory(
                    _dataBall.X,
                    _dataBall.Y,
                    _dataBall.Velocity.X * deltaTime,
                    _dataBall.Velocity.Y * deltaTime,
                    Radius
                );

                List<ICollision> collisions = [];

                // check wall collisions
                ICollision leftWallCollision = new WallCollision(_dataBall, trajectory, -1, 0, _left);
                leftWallCollision.AddToListIfColliding(collisions);

                ICollision rightWallCollision = new WallCollision(_dataBall, trajectory, -1, 0, _right);
                rightWallCollision.AddToListIfColliding(collisions);

                ICollision topWallCollision = new WallCollision(_dataBall, trajectory, 0, -1, _top);
                topWallCollision.AddToListIfColliding(collisions);

                ICollision bottomWallCollision = new WallCollision(_dataBall, trajectory, 0, -1, _bottom);
                bottomWallCollision.AddToListIfColliding(collisions);


                // check ball collisions
                foreach (LogicBall otherBall in balls)
                {
                    if (otherBall == this) continue;

                    ICollision ballCollision = new BallCollision(_dataBall, otherBall._dataBall, trajectory);
                    ballCollision.AddToListIfColliding(collisions);
                }

                if (collisions.Count == 0)
                {
                    _dataBall.Update(new Vector { X = trajectory.EndingX, Y = trajectory.EndingY }, null);
                    break;
                }

                ICollision? earliestCollision = collisions.MinBy(collision => collision.TPosition) ?? throw new Exception("Collisions list was empty, after checking that is it not");

                earliestCollision.PerformCollision();
                deltaTime = deltaTime * (1 - earliestCollision.TPosition);
            }

            UpdatePropertiesFromDataBall();
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
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
            Velocity = new Vector { X = _dataBall.Velocity.X * INVERSE_ASPECT_RATIO, Y = _dataBall.Velocity.Y };
            X = _dataBall.X * INVERSE_ASPECT_RATIO;
            Y = _dataBall.Y;
        }
    }
}
