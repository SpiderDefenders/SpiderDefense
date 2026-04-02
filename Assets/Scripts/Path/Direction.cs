using UnityEngine;

public enum Direction
{
    N,
    E,
    S,
    W,
    UNSPECIFIED
}

public static class DirectionExtensions
{
    public static Vector3 ToVector(this Direction dir)
    {
        return dir switch
        {
            Direction.N => Vector3.forward,
            Direction.E => Vector3.right,
            Direction.S => Vector3.back,
            Direction.W => Vector3.left,
            _ => Vector3.zero
        };
    }

    public static Quaternion ToRotationStraight(this Direction dir)
    {
        return dir switch
        {
            Direction.N => Quaternion.Euler(0, 0, 0),
            Direction.E => Quaternion.Euler(0, 90, 0),
            Direction.S => Quaternion.Euler(0, 180, 0),
            Direction.W => Quaternion.Euler(0, 270, 0),
            _ => Quaternion.identity
        };
    }

    public static Quaternion ToRotationTurn(this Direction from, Direction to)
    {
        return (from, to) switch
        {
            (Direction.N, Direction.E) or (Direction.W, Direction.S) => Quaternion.Euler(0, 180, 0),
            (Direction.E, Direction.S) or (Direction.N, Direction.W) => Quaternion.Euler(0, -90, 0),
            (Direction.S, Direction.W) or (Direction.E, Direction.N) => Quaternion.Euler(0, 0, 0),
            (Direction.W, Direction.N) or (Direction.S, Direction.E) => Quaternion.Euler(0, 90, 0),
            _ => Quaternion.identity
        };
    }

    public static Direction Opposite(this Direction dir)
    {
        return dir switch
        {
            Direction.N => Direction.S,
            Direction.E => Direction.W,
            Direction.S => Direction.N,
            Direction.W => Direction.E,
            _ => dir
        };
    }

    public static bool IsStraight(this Direction from, Direction to)
    {
        return from == to;
    }

    public static bool IsOpposite(this Direction from, Direction to)
    {
        return from.Opposite() == to;
    }
}