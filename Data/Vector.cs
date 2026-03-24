using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    internal class Vector : IVector
    {
        private double x;
        private double y;
        public double X { get => x; init => x = value; }
        public double Y { get => y; init => y = value; }

        public void FlipX()
        {
            x = -x;
        }

        public void FlipY()
        {
            y = -y;
        }
    }
}
