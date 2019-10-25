using UnityEngine;
using System.Collections;

public enum Direction {
    Start,
    Right,
    Front,
    Left,
    Back,
};
// <summary>
// Class for representing concrete maze cell.
// </summary>
public class MazeCell {
    public bool IsVisited = false;
    public bool WallRight = false;
    public bool WallFront = false;
    public bool WallLeft  = false;
    public bool WallBack  = false;
    public bool IsGoal    = false;

    public void
    Clear()
    {
        IsVisited = false;
        WallRight = false;
        WallFront = false;
        WallLeft  = false;
        WallBack  = false;
        IsGoal    = false;
    }
}
