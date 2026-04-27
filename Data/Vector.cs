namespace Data
{
    public class Vector : IVector
    {
        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// Mirror the velocity along the straight ax + by = 0
        /// </summary>
        /// <param name="a">Coefficient next to x</param>
        /// <param name="b">Coefficient next to y</param>
        public void MirrorAlongStraight(double a, double b)
        {
            double c = -b * X + a * Y;
            double a2B2 = a * a + b * b;

            X = 2 * (-b * c) / a2B2 - X;
            Y = 2 * (a * c) / a2B2 - Y;
        }
    }
}
