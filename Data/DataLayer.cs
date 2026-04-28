namespace Data
{
    public class DataLayer : IData
    {
        private readonly Random random = new Random();

        public DataLayer()
        {
        }


        public void Start(int ballCount, Action<IDataBall> upperLayerHandler)
        {
            for (int i = 0; i < ballCount; i++)
            {
                DataBall ball = new(random);

                upperLayerHandler(ball);
            }
        }
    }
}
