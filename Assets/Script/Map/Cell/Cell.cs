using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public int Row;
    public int Column;
    public bool Visited;

    public Dictionary<string, STATUS_DOOR> Doors;

    public Cell(int row, int col)
    {
        Row = row;
        Column = col;
        Doors = new Dictionary<string, STATUS_DOOR>
        {
            { GameConstants.Direction.Name.TOP,    STATUS_DOOR.CLOSE },
            { GameConstants.Direction.Name.BOTTOM, STATUS_DOOR.CLOSE },
            { GameConstants.Direction.Name.LEFT,   STATUS_DOOR.CLOSE },
            { GameConstants.Direction.Name.RIGHT,  STATUS_DOOR.CLOSE },
        };
    }

    public void SetApply() => Visited = true;

    public STATUS_DOOR GetStatusDoor(Vector2 direction)
    {
        if (direction == GameConstants.Direction.TOP) return Doors[GameConstants.Direction.Name.TOP];
        if (direction == GameConstants.Direction.BOTTOM) return Doors[GameConstants.Direction.Name.BOTTOM];
        if (direction == GameConstants.Direction.LEFT) return Doors[GameConstants.Direction.Name.LEFT];
        if (direction == GameConstants.Direction.RIGHT) return Doors[GameConstants.Direction.Name.RIGHT];
        return STATUS_DOOR.CLOSE;
    }
}

public enum STATUS_DOOR
{
    CLOSE = 0,
    OPEN = 1,
    BE_OPEN = 2
}
