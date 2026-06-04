using UnityEngine;

public interface IGrid
{
    int Columns { get; }
    int Rows { get; }
    void AddCell(Cell cell);
    void Setting(int columns, int rows);
    int CaculateIndex(Vector2 positionInGrid);
}

public interface IGrid<T> : IGrid
{
    T GetValue(int index);
    void SetValue(int index, T value);
    T GetNext(Vector2 direction);
}
