namespace Data
{
    internal class Ball : IBall
    {
        // The position of the ball is represented by the coordinates (x, y).
        // Which are normalized to the range [0, 1]
        // The same scale is used for the size of the ball.
        // To enable the room to have a different aspect ratio than 1:1, the radius of the ball is defined separately for x and y.
        private double x;
        private double y;

        private const double SIMULATION_ROOM_ASPECT_RATIO = 4.0 / 3.0;
        public double RADIUS_Y => 0.03;
        public double RADIUS_X => RADIUS_Y / SIMULATION_ROOM_ASPECT_RATIO;

        private const double MAX_RANDOM_VELOCITY = 0.3; // on one of the axes

        public event EventHandler<IVector>? NewPositionNotification;

        public double X
        {
            get => x;
            init
            {
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

        public IVector Velocity { get; init; }

        public Ball()
        {
            x = 0.5;
            y = 0.5;
            Velocity = new Vector { X = 0, Y = 0 };
        }


        /// <summary>
        /// Creates a ball at a random position in the simulation room with a random velocity.
        /// </summary>
        /// <param name="random"></param>
        public Ball(Random random)
        {
            x = RADIUS_X + (1 - 2 * RADIUS_X) * random.NextDouble();
            y = RADIUS_Y + (1 - 2 * RADIUS_Y) * random.NextDouble();

            Velocity = new Vector
            {
                X = (random.NextDouble() * 2 - 1) * MAX_RANDOM_VELOCITY,
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
            double newX = x + Velocity.X * deltaTime;
            double newY = y + Velocity.Y * deltaTime;

            while (!IsInBoundsX(newX))
            {
                Velocity.FlipX();
                if (newX < RADIUS_X) newX = 2 * RADIUS_X - newX;
                else newX = 2 - 2 * RADIUS_X - newX;
            }

            while (!IsInBoundsY(newY))
            {
                Velocity.FlipY();
                if (newY < RADIUS_Y) newY = 2 * RADIUS_Y - newY;
                else newY = 2 - 2 * RADIUS_Y - newY;
            }

            x = newX;
            y = newY;
            NewPositionNotification?.Invoke(this, new Vector { X = x, Y = y });
        }

        private bool IsInBoundsX(double coordinate)
        {
            if (coordinate > 1 - RADIUS_X) return false;
            if (coordinate < RADIUS_X) return false;
            return true;
        }

        private bool IsInBoundsY(double coordinate)
        {
            if (coordinate > 1 - RADIUS_Y) return false;
            if (coordinate < RADIUS_Y) return false;
            return true;
        }
    }
}