namespace LD53.AssetLogic;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
public class Maze
{
    public static Direction[] ValidSequence = new [] { Direction.Up, Direction.Up, Direction.Down, Direction.Down, Direction.Left, Direction.Right, Direction.Left, Direction.Right };
}