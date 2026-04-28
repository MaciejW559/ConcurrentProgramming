namespace Data
{
    public class DataLayer : IData
    {
        private bool disposed = false;
        private readonly Random random = new Random();
        private readonly List<DataBall> balls = []; // TODO delete this, unused

        public DataLayer()
        {
        }


        public void Start(int ballCount, Action<IDataBall> upperLayerHandler)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            for (int i = 0; i < ballCount; i++)
            {
                DataBall ball = new(random);

                upperLayerHandler(ball);
                balls.Add(ball);
            }
        }

        
        public void Dispose()
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
