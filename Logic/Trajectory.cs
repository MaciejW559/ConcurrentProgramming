using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Logic
{
    internal class Trajectory
    {
        public double StartingX { get; set; }
        public double StartingY { get; set; }

        public double EndingX { get; set; }
        public double EndingY { get; set; }

        public double Radious { get; set; }


        /// <summary>
        /// Find the collision point of the trajectory with a half-plane defined by
        /// ax + by + c >= 0
        /// 
        /// Returning -1 if there is no collision is a little scetchy,
        /// but considering the constraints on the ball's movement, it should be fine.
        /// Does not account for ball radious
        /// (balls center needs to be on the straight in the moment of collision, not the edge of the ball)
        /// </summary>
        /// <param name="a" x coefficient></param>
        /// <param name="b" y coefficient></param>
        /// <param name="c" free term></param>
        /// <returns>Progress t along the trajectory from Start to End</returns>
        public double CollideWithHalfPlane(double a, double b, double c)
        {
            if (a * StartingX + b * StartingY + c > 0)
            {
                throw new ArgumentException("The trajectory starts in the half-plane, which is an unintended edge case");
            }

            if (a * EndingX + b * EndingY + c < 0)
            {
                // The trajectory does not end in the half-plane,
                // so, since the trajectory is a straight line, it does not collide with the half-plane at all.
                return -1;
            }

            double d1 = a * StartingX + b * StartingY + c;
            double d2 = a * EndingX + b * EndingY + c;
            return d1 / (d1 - d2);

            
        }

        public IVector ProgressTToPoint(double t)
        {
            return new Vector()
            {
                X = StartingX + t * (EndingX - StartingX),
                Y = StartingY + t * (EndingY - StartingY)
            };
        } 
    }
}
