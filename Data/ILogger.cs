using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public interface ILogger
    {
        void Log<T>(T message);
    }
}
