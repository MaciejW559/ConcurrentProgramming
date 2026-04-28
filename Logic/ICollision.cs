namespace Logic;

internal interface ICollision
{
    /// <summary>
    /// Get the position of the collision along the trajectory,
    /// where 0 is the start of the trajectory and 1 is the end of the trajectory.
    /// </summary>
    double TPosition { get; }

    /// <summary>
    /// Perform the actual collision by moving the ball to the collision point
    /// and changing its velocity according to the collision.
    /// </summary>
    void PerformCollision();

    void AddToListIfColliding(List<ICollision> collisions)
    {
        if (TPosition >= 0 && TPosition <= 1)
        {
            collisions.Add(this);
        }
    }
}
