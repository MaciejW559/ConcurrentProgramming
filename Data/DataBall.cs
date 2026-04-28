using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    /// <summary>
    /// The position of the ball is represented by the coordinates (x, y).
    /// [0, 0] is the top left corner of the simulation room, [aspectRatio, 1] is the bottom right corner.
    /// Keeping the coordinates within the proper range is not the responsibility of this class, see LogicBall instead
    /// </summary>
    public class DataBall : IDataBall
    {
        /// <summary>
        /// in the range [0, SIMULATION_ROOM_ASPECT_RATIO]
        /// </summary>
        public double X { get; private set; }

        /// <summary>
        /// in the range [0, 1]
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// The same scale is used for the radious. Radious of 0.05 means 1/20th of the height of the simulation room
        /// </summary>
        public double Radius => 0.03;
        public double Weight => 1.0;


        /// <summary>
        /// Maximum random initial velocity on each of the axis individually
        /// </summary>
        private const double MAX_RANDOM_VELOCITY = 0.3;


        /// <summary>
        /// Ball Velocity is in the same scale as position and radious
        /// Velocity.X of 0.5, means the ball will travel half the simulation room in 1s
        /// _velocity is a private, mutable Vector
        /// </summary>
        private Vector _velocity { get; set; }

        /// <summary>
        /// Velocity is the same object cast to the IVector, immutable type
        /// </summary>
        public IVector Velocity {
            get => _velocity;
            init => _velocity = new Vector() { X = value.X, Y = value.Y }; 
        }

        /// <summary>
        /// For the INotifyPropertyChanged interface
        /// Due to the fact that both x and y coordinates often change together and are stored in different properties,
        /// an event with null as an argument is raised, instead of raising two events for X and Y, to signal that both of them have changed. See OnPropertyChanged method.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        public DataBall()
        {
            X = 0.5;
            Y = 0.5;
            _velocity = new Vector { X = 0, Y = 0 };
        }

      public DataBall(double x, double y, Vector velocity)
        {
            X = x;
            Y = y;
            _velocity = velocity;
        }


        /// <summary>
        /// Creates a ball at a random position in the simulation room with a random velocity.
        /// </summary>
        /// <param name="random"></param>
        public DataBall(Random random)
        {
            X = Radius + (1 - 2 * Radius) * random.NextDouble();
            Y = Radius + (1 - 2 * Radius) * random.NextDouble();

            _velocity = new Vector
            {
                X = (random.NextDouble() * 2 - 1) * MAX_RANDOM_VELOCITY,
                Y = (random.NextDouble() * 2 - 1) * MAX_RANDOM_VELOCITY,
            };
        }


        /// <summary>
        /// Update the balls position and velocity
        /// </summary>
        /// <param name="newPosition">New position vector in the internal bounds</param>
        /// <param name="newVelocity">New velocity vector</param>
        public void Update(IVector? newPosition, IVector? newVelocity)
        {
            if (newPosition != null)
            {
                X = newPosition.X;
                Y = newPosition.Y;
            }
            if (newVelocity != null)
            {
                _velocity = new Vector { X = newVelocity.X, Y = newVelocity.Y };
            }
            OnPropertyChanged(null);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}