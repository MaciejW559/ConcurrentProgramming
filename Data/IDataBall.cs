namespace Data;

public interface IDataBall : IBall
{

    void Update(IVector? newPosition, IVector? newVelocity);
}
