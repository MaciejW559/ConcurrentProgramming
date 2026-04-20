namespace Data
{
    internal class Ball : IBall
    {
        // The position of the ball is represented by the coordinates (x, y).
        // Which are normalized to the range [0, 1]
        // The same scale is used for the size of the ball, which is represented by a constant radius.
        // which means the ball can have at most a RADIUS of 0.5 to fit within the normalized coordinate system.
        private double x;
        private double y;

         private const double RADIUS_Y = 0.03;
        private const double RADIUS_X = RADIUS_Y * (3.0 / 4.0);

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

        public Ball(Random random)
        {
            x = RADIUS_X + (1 - 2 * RADIUS_X) * random.NextDouble();
            y = RADIUS_Y + (1 - 2 * RADIUS_Y) * random.NextDouble();

            Velocity = new Vector
            {
                X = (random.NextDouble() * 2 - 1) * 0.0001,
                Y = (random.NextDouble() * 2 - 1) * 0.0001,
            };
        }

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