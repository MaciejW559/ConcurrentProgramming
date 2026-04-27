using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public interface IDataBall : IBall
    {
        void MirrorAlongStraight(double a, double b, double c);

        void Move(double deltaTime);
    }
}
