namespace Data;

public interface IDataBall : IBall
{

    void Update(IVector? newPosition, IVector? newVelocity);

    public static DataBall FromNormalizedCoords(double normalizedX, double normalizedY, Vector velocity)
    {
        return new DataBall(
            normalizedX * IData.SIMULATION_ROOM_ASPECT_RATIO,
            normalizedY,
            new Vector { X = velocity.X * IData.SIMULATION_ROOM_ASPECT_RATIO, Y = velocity.Y}
            );
    }
}
