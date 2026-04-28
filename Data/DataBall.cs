using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    /// <summary>
    /// The position of the ball is represented by the coordinates (x, y).
    /// [0, 0] is the top left corner of the simulation room, [1, 1] is the bottom right corner.
    /// Keeping the coordinates within the proper range is not the responsibility of this class, see LogicBall instead
    /// </summary>
    public class DataBall : IDataBall
    {
        private static readonly double INVERSE_ASPECT_RATIO = 1.0 / IData.SIMULATION_ROOM_ASPECT_RATIO;


        /// <summary>
        /// Internally x is in the range [0, SIMULATION_ROOM_ASPECT_RATIO] to escape the elipse madness
        /// </summary>
        private double x;

        /// <summary>
        /// y is in the range [0, 1]
        /// </summary>
        private double y;

        /// <summary>
        /// Internal x value, normalized to [0, 1]
        /// </summary>
        public double X
        {
            get => x * INVERSE_ASPECT_RATIO;
            init => x = value * IData.SIMULATION_ROOM_ASPECT_RATIO;
        }

        /// <summary>
        /// Internal y value
        /// </summary>
        public double Y
        {
            get => y;
            init => y = value;
        }

        /// <summary>
        /// The same scale is used for the radious. Radious of 0.05 means 1/20th of the height of the simulation room
        /// </summary>
        public double RADIUS => 0.03;
        public double WEIGHT => 1.0;


        /// <summary>
        /// Maximum random initial velocity on each of the axis individually
        /// </summary>
        private const double MAX_RANDOM_VELOCITY = 0.3;


        /// <summary>
        /// Ball Velocity is in the same scale as position and radious
        /// Velocity.X of 0.5, means the ball will travel half the simulation room in 1s
        /// velocity is a private, mutable Vector
        /// </summary>
        private Vector velocity { get; init; }

        /// <summary>
        /// Velocity is the same object cast to the IVector, immutable type
        /// </summary>
        public IVector Velocity { 
            get => velocity;
            init => velocity = new Vector() { X = value.X, Y = value.Y }; 
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
            velocity = new Vector { X = 0, Y = 0 };
        }


        /// <summary>
        /// Creates a ball at a random position in the simulation room with a random velocity.
        /// </summary>
        /// <param name="random"></param>
        public DataBall(Random random)
        {
            X = RADIUS + (1 - 2 * RADIUS) * random.NextDouble();
            Y = RADIUS + (1 - 2 * RADIUS) * random.NextDouble();

            velocity = new Vector
            {
                X = (random.NextDouble() * 2 - 1) * MAX_RANDOM_VELOCITY * INVERSE_ASPECT_RATIO,
                Y = (random.NextDouble() * 2 - 1) * MAX_RANDOM_VELOCITY,
            };
        }

        /// <summary>
        /// Move the ball according to its velocity and the time elapsed since the last movement in seconds.
        /// Does not bounce off walls, just moves in a straight line. See LogicBall for the bouncing logic.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Move(double deltaTime)
        {
            x += IData.SIMULATION_ROOM_ASPECT_RATIO * velocity.X * deltaTime;
            y += velocity.Y * deltaTime;

            OnPropertyChanged(null);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Mirror the balls position and velocity along the straight ax + by + c = 0
        /// In the normalized scale [0, 1], meaning right wall is 1x + 0y = 1
        /// 
        /// </summary>
        /// <param name="a">Coefficient next to x</param>
        /// <param name="b">Coefficient next to y</param>
        /// <param name="c">Free term</param>
        public void MirrorAlongStraight(double a, double b, double c)
        {
            // Perpendicular line to ax + by + c = 0 is bx - ay + c' = 0,
            // where c' is calculated based on the fact that the line goes through the point (x, y)
            a *= INVERSE_ASPECT_RATIO;
            double cPrim = -b * x + a * y;

            double a2B2 = a * a + b * b;

            double mirrorPointX = (-b * cPrim - a * c) / a2B2;
            double mirrorPointY = (-b * c + a * cPrim) / a2B2;

            x = 2 * mirrorPointX - x;
            y = 2 * mirrorPointY - y;


            velocity.MirrorAlongStraight(a, b);

            OnPropertyChanged(null);
        }
    }
}