using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Logic
{
    public abstract class LogicAbstractAPI : IDisposable
    {
        protected static readonly double FPS = 60;

        private static Lazy<LogicAbstractAPI> modelInstance = new Lazy<LogicAbstractAPI>(() => new LogicLayerImplementation());
        public static LogicAbstractAPI GetLogicLayer()
        {
            return modelInstance.Value;
        }
        public abstract void Dispose();

        #region public API

        public abstract void Start(int ballCount, Action<IBall> upperLayerHandler);

        public abstract Task SequentialMainLoop();

        #endregion public API
    }
}
