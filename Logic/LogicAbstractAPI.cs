using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Logic
{
    internal abstract class LogicAbstractAPI : IDisposable
    {
        private static Lazy<LogicAbstractAPI> modelInstance = new Lazy<LogicAbstractAPI>(() => new LogicLayerImplementation());
        public static LogicAbstractAPI GetLogicLayer()
        {
            return modelInstance.Value;
        }
        public abstract void Dispose();

        #region public API

        public abstract void Start(int ballCount, Action<IBall> upperLayerHandler);

        #endregion public API
    }
}
