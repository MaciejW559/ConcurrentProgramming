namespace Data
{
    internal class DataLayerImplementation : DataAbstractAPI
    {
        private bool disposed = false;
        private Random random = new Random();
        private List<Ball> balls = new List<Ball>();

        public DataLayerImplementation()
        {
        }


        public override void Start(int ballCount, Action<IBall> upperLayerHandler)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            for (int i = 0; i < ballCount; i++)
            {
                Ball ball = new Ball(random);

                upperLayerHandler(ball);
                balls.Add(ball);
            }
        }


        public override void Move(double deltaTime)
        {
            foreach (var ball in balls)
            {
                ball.Move(deltaTime);
            }
        }
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                balls.Clear();
            }
            disposed = true;
        }
    }
}
