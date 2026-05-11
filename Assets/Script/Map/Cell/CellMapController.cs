using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMapController : MonoBehaviour, IMapController<CellController>
{
    [SerializeField] CellController prefabObject;
    [SerializeField] List<CellController> _cellController;
    [SerializeField] CellController _current, _next;
    public int Columns { get; private set; }
    public int Rows { get; private set; }
    public void AddCell(Cell cell)
    {
        CellController cellController = Instantiate(prefabObject, this.transform) as CellController;
        cellController.AddCell(cell);
        _cellController.Add(cellController);
    }

    public CellController GetValue(int index)
    {
        var cellController = _cellController[index];
        return cellController;
    }

    public void SetValue(int index, CellController cellController)
    {
        _cellController[index] = cellController;
    }

    public void Setting(int Columns, int Rows)
    {
        this.Columns = Columns;
        this.Rows = Rows;
        _current = this._cellController[0];
    }

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAP, OnLoadMap);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_LOAD_MAP, OnLoadMap);
    }


    public CellController GetStart()
    {
        var start = GetValue(0);
        return start;
    }
    private void OnLoadMap(object obj = null)
    {
        _current = _next;
        _next = null;
    }

    public CellController GetNext(Vector2 direction)
    {
        direction.y = -direction.y;
        var positionNextRoom = _current.GetGridPosition() + direction;
        int index = (int)positionNextRoom.y * this.Columns + (int)positionNextRoom.x;
        _next = GetValue(index);
        direction.y = -direction.y;
        _next.GetStartDoorPosition(-direction);
        _current.UpdateStatusDoor(direction);

        return _next;
    }
}
