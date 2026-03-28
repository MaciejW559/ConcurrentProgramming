namespace Data
{
    public class Ball : IBall
    {
        // The position of the ball is represented by the coordinates (x, y).
        // Which are normalized to the range [0, 1]
        // The same scale is used for the size of the ball, which is represented by a constant radius.
        // which means the ball can have at most a radious of 0.5 to fit within the normalized coordinate system.
        private double x;
        private double y;
        private const double RADIOUS = 0.03;

        public event EventHandler<IVector>? NewPositionNotification;

        public double X {
            get => x;
            init {
                if (IsInBounds(value)) x = value;
                
            }
        }
        public double Y {
            get => y;
            init
            {
                if (IsInBounds(value)) y = value;
            }
        }

        public IVector Velocity { get; init; }


        public Ball() {
            X = 0.5;
            Y = 0.5;
            Velocity = new Vector
            {
                X = 0,
                Y = 0
            };
        }

        public Ball(Random random)
        {
            X = RADIOUS + (1 - 2 * RADIOUS) * random.NextDouble();
            Y = RADIOUS + (1 - 2 * RADIOUS) * random.NextDouble();
            Velocity = new Vector
            {
                X = random.NextDouble(),
                Y = random.NextDouble()
            };
        }


        // niezgodne z wymaganiami, bo nie chciało mi się bawić w przesuwanie losowe, jak można zrobić od razu odbijanie się od ścian
        public void Move(double deltaTime)
        {
            // a bunch of repeating code
            double newX = X + Velocity.X * deltaTime;
            double newY = Y + Velocity.Y * deltaTime;
            while (!IsInBounds(newX))
            {
                Velocity.FlipX();
                if (newX < RADIOUS) newX = 2 * RADIOUS - newX;
                else newX = 2 - 2 * RADIOUS - newX;
            }

            while (!IsInBounds(newY))
            {
                Velocity.FlipY();
                if (newY < RADIOUS) newY = 2 * RADIOUS - newY;
                else newY = 2 - 2 * RADIOUS - newY;
            }

            x = newX;
            y = newY;
            NewPositionNotification?.Invoke(this, new Vector { X = x, Y = y});

        }

        private bool IsInBounds(double coordiante)
        {
            if (coordiante > 1 - RADIOUS) return false;
            if (coordiante < RADIOUS) return false;
            return true;
        }

    }
}
