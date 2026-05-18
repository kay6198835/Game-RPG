using UnityEngine;

public interface IGridItem
{
    void AddCell(Cell cell);
    Vector2 GetGridPosition();
}
