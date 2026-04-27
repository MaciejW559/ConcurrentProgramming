namespace Data
{
    internal class DataLayerImplementation : DataAbstractAPI
    {
        private bool disposed = false;
        private Random random = new Random();
        private List<DataBall> balls = new List<DataBall>();

        public DataLayerImplementation()
        {
        }


        public override void Start(int ballCount, Action<IDataBall> upperLayerHandler)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            for (int i = 0; i < ballCount; i++)
            {
                DataBall ball = new DataBall(random);

                upperLayerHandler(ball);
                balls.Add(ball);
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
