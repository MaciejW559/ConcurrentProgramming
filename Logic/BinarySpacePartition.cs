using System.Collections.ObjectModel;
using Data;

namespace Logic;

internal class BinarySpacePartition
{
    // partition by inequality ax + by + c > 0, where a and b are not both zero
    // and inequalities from parent partitions
    private double a;
    private double b;
    private double c;
    private readonly BinarySpacePartition? subPartition1;
    private readonly BinarySpacePartition? subPartition2;
    private readonly Lock _lock = new();
    private readonly Collection<LogicBall> ballsInPartition = [];

    private static int DEPTH = 1;

    public static readonly BinarySpacePartition masterPartition = new BinarySpacePartition(
        1, 0, 0,
        0, 1, 0,
        DEPTH
    );


    private BinarySpacePartition(double parentA, double parentB, double parentC, double a, double b, double c, int depth)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        // no more subpartitions
        if (depth <= 0) return;

        if (a != 0) // vertical line
        {
            // parent had division by a horizontal line
            // if parent's b was 1, then it was 1y + parentC > 0,
            // so the childs line should be the same, but shifted more right
            // otherwise, it was -1y + parentC > 0 <=> 1y - parentC < 0,
            // so the childs line should be the same, but shifted more left
            double newC = parentB == 1 ? (parentC + 1) / 2 : parentC / 2;

            subPartition1 = new BinarySpacePartition(a, b, c, 0, 1, -newC, depth - 1);
            subPartition2 = new BinarySpacePartition(a, b, c, 0, -1, newC, depth - 1);
        }
        else // horizontal line
        {
            // same logic as before, but with x and y swapped
            double newC = parentA == 1 ? (parentC + 1) / 2 : parentC / 2;
            subPartition1 = new BinarySpacePartition(a, b, c, 1, 0, -newC, depth - 1);
            subPartition2 = new BinarySpacePartition(a, b, c, -1, 0, newC, depth - 1);
        }
    }

    /// <summary>
    /// Lock all leaf subpartitions in the binary tree
    /// Doesn't lock non leafs, bc if two partitions collide, they share leafs
    /// </summary>
    /// <param name="action"></param>
    /// <exception cref="SystemException"></exception>
    public void LockPartition(Action action)
    {
        if (subPartition1 == null && subPartition2 == null)
        {
            lock (_lock) action();
            return;
        }
        if (subPartition1 == null || subPartition2 == null)
        {
            throw new SystemException("Binary Space Partition can't have exactly one child partition");
        }

        subPartition1.LockPartition(
            () => subPartition2.LockPartition(action)
        );
    }

    public bool CouldCollideWithPartition(Trajectory trajectory)
    {
        return trajectory.CouldCollideWithHalfPlane(a, b, c);
    }

    public bool CouldPointCollideWithPartition(double x, double y, double radious)
    {
        return Trajectory.CouldPosCollideWithHalfPlane(a, b, c, x, y, radious);
    }


    /// <summary>
    /// Finds the minimal fragment of the board, the ball can interact with
    /// and locks it
    /// </summary>
    /// <param name="action"></param>
    /// <param name="trajectory"></param>
    /// <exception cref="SystemException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void FindAndLockMinimalPartition(Action<IEnumerable<LogicBall>> action, Trajectory trajectory)
    {
        if (subPartition1 == null && subPartition2 == null)
        {
            LockPartition(() => action(ballsInPartition));
            return;
        }
        if (subPartition1 == null || subPartition2 == null)
        {
            throw new SystemException("Binary Space Partition can't have exactly one child partition");
        }

        bool collidesWithSub1 = subPartition1.CouldCollideWithPartition(trajectory);
        bool collidesWithSub2 = subPartition2.CouldCollideWithPartition(trajectory);

        if (collidesWithSub1 && collidesWithSub2)
        {
            LockPartition(() => action(GetBalls()));
            return;
        }

        if (collidesWithSub1)
        {
            subPartition1.FindAndLockMinimalPartition(action, trajectory);
            return;
        }
        if (collidesWithSub2)
        {
            subPartition2.FindAndLockMinimalPartition(action, trajectory);
            return;
        }
        throw new ArgumentException("Trajectory doesn't collide with Partition");
    }


    /// <summary>
    /// Register ball in the lists of balls in each partition according to its current position
    /// </summary>
    /// <param name="currentX"></param>
    /// <param name="currentY"></param>
    /// <returns>Unregister Action</returns>
    public Action RegisterBallInPartitioningSystem(LogicBall ball)
    {
        if (subPartition1 == null && subPartition2 == null)
        {
            ballsInPartition.Add(ball);
            return () => ballsInPartition.Remove(ball);
        }
        if (subPartition1 == null || subPartition2 == null)
        {
            throw new SystemException("Binary Space Partition can't have exactly one child partition");
        }

        bool collidesWithSub1 = subPartition1.CouldPointCollideWithPartition(ball.X, ball.Y, ball.Radius);
        bool collidesWithSub2 = subPartition2.CouldPointCollideWithPartition(ball.X, ball.Y, ball.Radius);

        if (collidesWithSub1 && collidesWithSub2)
        {
            ballsInPartition.Add(ball);
            return () => ballsInPartition.Remove(ball);
        }

        if (collidesWithSub1)
        {
            return subPartition1.RegisterBallInPartitioningSystem(ball);
        }
        if (collidesWithSub2)
        {
            return subPartition2.RegisterBallInPartitioningSystem(ball);
        }
        throw new ArgumentException("Trajectory doesn't collide with Partition");

    }

    public IEnumerable<LogicBall> GetBalls()
    {
        if (subPartition1 == null && subPartition2 == null)
        {
            return ballsInPartition;
        }
        if (subPartition1 == null || subPartition2 == null)
        {
            throw new SystemException("Binary Space Partition can't have exactly one child partition");
        }
        return subPartition1.GetBalls().Concat(subPartition2.GetBalls());
    }
}
