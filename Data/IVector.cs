namespace Data
{
    public interface IVector
    {
        double X { get; }
        double Y { get; }

        double Dot(IVector other) => X * other.X + Y * other.Y;
    }
}
