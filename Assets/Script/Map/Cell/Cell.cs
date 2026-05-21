using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Cell
{
    public int Row;
    public int Column;
    public bool Visited;

    public STATUS_DOOR Top;
    public STATUS_DOOR Bottom;
    public STATUS_DOOR Right;
    public STATUS_DOOR Left;
    public Cell(int row, int col)
    {
        Row = row;
        Column = col;
    }

    public void SetApply()
    {
        this.Visited = true;
    }
    public void GetStatusDoor(Vector2 direction)
    {
        Utility.Instance.ToCardinalDirection(direction);
    }
}
public enum STATUS_DOOR
{
    CLOSE = 0,
    OPEN = 1,
    BE_OPEN = 2
}