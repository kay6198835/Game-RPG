using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Cell
{
    public int Row;
    public int Column;
    public bool Visited;

    public int Top;
    public int Bottom;
    public int Right;
    public int Left;
    public Cell(int row, int col)
    {
        Row = row;
        Column = col;
    }

    public void SetApply()
    {
        this.Visited = true;
    }
}
public enum STATUS_DOOR
{
    CLOSE = 0,
    OPEN = 1,
    BE_OPEN = 2
}