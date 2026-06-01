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
            { GameConstants.Direction.Name.TOP,    STATUS_DOOR.DISABLE },
            { GameConstants.Direction.Name.BOTTOM, STATUS_DOOR.DISABLE },
            { GameConstants.Direction.Name.LEFT,   STATUS_DOOR.DISABLE },
            { GameConstants.Direction.Name.RIGHT,  STATUS_DOOR.DISABLE },
        };
    }

    public void SetApply() => Visited = true;

    public STATUS_DOOR GetStatusDoor(Vector2 direction)
    {
        if (direction == GameConstants.Direction.TOP) return Doors[GameConstants.Direction.Name.TOP];
        if (direction == GameConstants.Direction.BOTTOM) return Doors[GameConstants.Direction.Name.BOTTOM];
        if (direction == GameConstants.Direction.LEFT) return Doors[GameConstants.Direction.Name.LEFT];
        if (direction == GameConstants.Direction.RIGHT) return Doors[GameConstants.Direction.Name.RIGHT];
        return STATUS_DOOR.DISABLE;
    }
}

public enum STATUS_DOOR
{
    DISABLE = 0, // No door exists in this direction.
    ENEBLE = 1, // The door is enabled but not yet open.
    // Must ENABLE first to 
    BE_OPEN = 2, // Like a door that is already open, but not yet passed through by the player. This status can be used to prevent the player from immediately closing the door after opening it. (For CellMap)
    OPEN = 3, // The door is open and can be passed through by the player. (For RoomCell)
    CLOSE = 4, // The door is closed and cannot be passed through by the player. (For RoomCell)

}
