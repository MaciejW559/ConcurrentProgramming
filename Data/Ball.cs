using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Data
{
    internal class Ball : IBall
    {
        // The position of the ball is represented by the coordinates (x, y).

        // Internally x is in the range [0, SIMULATION_ROOM_ASPECT_RATIO] and y in [0, 1]
        // to escape the elipse madness

        // They are both normalized to [0, 1] in X and Y properties

        // The same scale is used for the radious. Radious of 0.05 means 1/20th of the height of the simulation room
        private double x;
        private double y;

        private const double SIMULATION_ROOM_ASPECT_RATIO = 4.0 / 3.0;
        public double RADIUS => 0.03;

        private const double MAX_RANDOM_VELOCITY = 0.3; // on one of the axes

        public event PropertyChangedEventHandler? PropertyChanged;

        public double X
        {
            get => x / SIMULATION_ROOM_ASPECT_RATIO;
            init
            {
                value *= SIMULATION_ROOM_ASPECT_RATIO;
                if (IsInBoundsX(value)) x = value;
            }
        }
        public double Y
        {
            get => y;
            init
            {
                if (IsInBoundsY(value)) y = value;
            }
        }

        // Ball velocity is in the same scale as position and radious
        // Velocity.X of 0.5, means the ball will travel half the simulation room in 1s
        public IVector Velocity { get; init; }

        public Ball()
        {
            X = 0.5;
            Y = 0.5;
            Velocity = new Vector { X = 0, Y = 0 };
        }


        /// <summary>
        /// Creates a ball at a random position in the simulation room with a random velocity.
        /// </summary>
        /// <param name="random"></param>
        public Ball(Random random)
        {
            X = RADIUS + (1 - 2 * RADIUS) * random.NextDouble();
            Y = RADIUS + (1 - 2 * RADIUS) * random.NextDouble();

            Velocity = new Vector
            {
                X = (random.NextDouble() * 2 - 1) * MAX_RANDOM_VELOCITY / SIMULATION_ROOM_ASPECT_RATIO,
                Y = (random.NextDouble() * 2 - 1) * MAX_RANDOM_VELOCITY,
            };
        }

        /// <summary>
        /// Move the ball according to its velocity and the time elapsed since the last movement in seconds.
        /// Includes bouncing off the walls of the simulation room, which are located at x = 0, x = 1, y = 0 and y = 1.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Move(double deltaTime)
        {
            double newX = x + SIMULATION_ROOM_ASPECT_RATIO * Velocity.X * deltaTime;
            double newY = y + Velocity.Y * deltaTime;

            while (!IsInBoundsX(newX))
            {
                Velocity.FlipX();
                if (newX < RADIUS) newX = 2 * RADIUS - newX;
                else newX = 2 * SIMULATION_ROOM_ASPECT_RATIO - 2 * RADIUS - newX;
            }

            while (!IsInBoundsY(newY))
            {
                Velocity.FlipY();
                if (newY < RADIUS) newY = 2 * RADIUS - newY;
                else newY = 2 - 2 * RADIUS - newY;
            }

            x = newX;
            y = newY;
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        


        private bool IsInBoundsX(double coordinate)
        {
            if (coordinate > SIMULATION_ROOM_ASPECT_RATIO - RADIUS) return false;
            if (coordinate < RADIUS) return false;
            return true;
        }

        private bool IsInBoundsY(double coordinate)
        {
            if (coordinate > 1 - RADIUS) return false;
            if (coordinate < RADIUS) return false;
            return true;
        }
    }
}