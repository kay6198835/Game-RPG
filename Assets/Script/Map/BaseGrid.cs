using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseGrid<T> : MonoBehaviour, IGrid<T>
    where T : MonoBehaviour, IGridItem
{
    [SerializeField] protected T prefabObject;
    [SerializeField] protected List<T> _list = new List<T>();
    [SerializeField] protected T _current;
    [SerializeField] protected T _next;

    public int Columns { get; private set; }
    public int Rows { get; private set; }

    public void AddCell(Cell cell)
    {
        T item = Instantiate(prefabObject, transform);
        item.name = "Room_" + _list.Count;
        item.AddCell(cell);
        _list.Add(item);
    }

    public virtual void Setting(int columns, int rows)
    {
        Columns = columns;
        Rows = rows;
    }

    public T GetValue(int index) => _list[index];
    public void SetValue(int index, T value) => _list[index] = value;

    public T GetNext(Vector2 direction)
    {
        direction.y = -direction.y;
        var positionNextRoom = _current.GetGridPosition() + direction;
        int index = this.CaculateIndex(positionNextRoom);
        _next = GetValue(index);
        return _next;
    }

    public int CaculateIndex(Vector2 positionInGrid)
    {
        return (int)positionInGrid.y * Columns + (int)positionInGrid.x;
    }
}
