using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Data
{
    internal class DataLayerImplementation : DataAbstractAPI
    {
        private bool disposed = false;
        private readonly System.Timers.Timer clock;
        private Random random = new Random();
        private List<Ball> balls = new List<Ball>();

        public DataLayerImplementation(double fps)
        {
            clock = new System.Timers.Timer(TimeSpan.FromSeconds(1 / fps));
            clock.Elapsed += (object? sender, ElapsedEventArgs e) => Move();
            clock.AutoReset = true;
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (disposing)
            {
                clock.Dispose();
                balls.Clear();
            }
            disposed = true;
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

            clock.Start();
        }


        private void Move()
        {
            foreach (var ball in balls)
            {
                ball.Move(clock.Interval);
            }
        }
    }
}
