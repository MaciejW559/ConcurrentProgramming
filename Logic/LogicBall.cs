using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Data;

namespace Logic
{
    internal class LogicBall : IBall
    {
        private static readonly Lock _moveLock = new();
        private readonly Lock _propertyLock = new();


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
        public double X
        {
            get
            {
                lock (_propertyLock) return field;
            }
            private set
            {
                lock (_propertyLock) field = value;
            }
        }
        /// <summary>
        /// Y coordinate normalized to [0, 1]
        /// </summary>
        public double Y
        {
            get
            {
                lock (_propertyLock) return field;
            }
            private set
            {
                lock (_propertyLock) field = value;
            }
        }
        /// <summary>
        /// Normalized velocity
        /// </summary>
        public IVector Velocity
        {
            get
            {
                lock (_propertyLock) return field;
            }
            private set
            {
                lock (_propertyLock) field = value;
            }
        }


        public double Radius => _dataBall.Radius;
        public double Weight => _dataBall.Weight;

        private readonly ILogger? _logger;
        private readonly IDataBall _dataBall;
        private bool _midMovement = false;
        private bool _movedSince = false;

        public LogicBall(IDataBall dataBall, ILogger? logger = null)
        {
            _dataBall = dataBall;
            _logger = logger;

            _left = Radius;
            _right = IData.SIMULATION_ROOM_ASPECT_RATIO - _left;

            _top = Radius;
            _bottom = 1 - _top;

            // if it helps VS not recognizing the same damn line is in UpdatePropertiesFromDataBall and raising "fied not initialized" warning, then so be it
            Velocity = _dataBall.Velocity;
            UpdatePropertiesFromDataBall();

            if (!IsInBoundsX(dataBall.X)) throw new ArgumentException("Initial Databall position out of bounds");
            if (!IsInBoundsY(dataBall.Y)) throw new ArgumentException("Initial Databall position out of bounds");

            _dataBall.PropertyChanged += DataBall_PropertyChanged;
        }

        public async Task RunSimulationLoopAsync(Collection<LogicBall> allBalls, CancellationToken token, Barrier barrier)
        {

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1.0 / ILogic.FPS));
            var timestamp = Stopwatch.GetTimestamp();

            try
            {
                while (await timer.WaitForNextTickAsync(token))
                {
                    var elapsed = Stopwatch.GetElapsedTime(timestamp);
                    timestamp = Stopwatch.GetTimestamp();

                    Move(elapsed.TotalSeconds, allBalls);
                    barrier.SignalAndWait(token);
                }
            }
            catch (OperationCanceledException)
            {
                // expected, token triggered cancellation
            }
        }


        private void DataBall_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_midMovement)
            {
                _movedSince = true;
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
        public void Move(double deltaTime, Collection<LogicBall> balls)
        {
            while (deltaTime > 0)
            {
                _midMovement = true;
                deltaTime = MoveIteration(deltaTime, balls);
                _midMovement = false;
                UpdatePropertiesFromDataBall();
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Y));
            }
        }

        private double MoveIteration(double deltaTime, Collection<LogicBall> balls)
        {
            List<ICollision> collisions = [];
            Trajectory trajectory;
            lock (_moveLock)
            {
                // effectively copy movement parameters into the Trajectory
                _movedSince = false;
                trajectory = new Trajectory(
                    _dataBall,
                    deltaTime
                );
            }

            // asynchronously check for wall collisions, while the lock is released
            CheckWallCollisions(trajectory, collisions);


            lock (_moveLock)
            {
                if (_movedSince)
                {
                    // need to recalculate trajectory, since the ball has collided in the meantime
                    collisions.Clear();
                    trajectory = new Trajectory(
                        _dataBall,
                        deltaTime
                    );

                    CheckWallCollisions(trajectory, collisions);
                }

                // check ball collisions
                foreach (LogicBall otherBall in balls)
                {
                    if (otherBall == this) continue;

                    ICollision ballCollision = new BallCollision(_dataBall, otherBall._dataBall, trajectory);
                    ballCollision.AddToListIfColliding(collisions);
                }

                if (collisions.Count == 0)
                {
                    // no collisions, make the full movement and end the iteration
                    _dataBall.Update(new Vector { X = trajectory.EndingX, Y = trajectory.EndingY }, null);
                    return 0;
                }

                ICollision? earliestCollision = collisions.MinBy(collision => collision.TPosition) ?? throw new Exception("Collisions list was empty, after checking that is it not");

                earliestCollision.PerformCollision();

                _logger?.Log(new
                {
                    Event = "Collision",
                    Type = earliestCollision.GetType().Name,
                    BallVelocityX = Velocity.X,
                    BallVelocityY = Velocity.Y
                });

                return deltaTime * (1 - earliestCollision.TPosition);
            }
        }

        private void CheckWallCollisions(Trajectory trajectory, List<ICollision> collisionsList)
        {
            ICollision leftWallCollision = new WallCollision(_dataBall, trajectory, -1, 0, _left);
            leftWallCollision.AddToListIfColliding(collisionsList);

            ICollision rightWallCollision = new WallCollision(_dataBall, trajectory, 1, 0, -_right);
            rightWallCollision.AddToListIfColliding(collisionsList);

            ICollision topWallCollision = new WallCollision(_dataBall, trajectory, 0, -1, _top);
            topWallCollision.AddToListIfColliding(collisionsList);

            ICollision bottomWallCollision = new WallCollision(_dataBall, trajectory, 0, 1, -_bottom);
            bottomWallCollision.AddToListIfColliding(collisionsList);
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
            lock (_propertyLock)
            {
                Velocity = new Vector { X = _dataBall.Velocity.X * INVERSE_ASPECT_RATIO, Y = _dataBall.Velocity.Y };
                X = _dataBall.X * INVERSE_ASPECT_RATIO;
                Y = _dataBall.Y;
            }
        }
    }
}