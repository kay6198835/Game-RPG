using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MazeController : MonoBehaviour
{
    [SerializeField] public int Rows = 3;
    [SerializeField] public int Columns = 3;
    [SerializeField] MazeGenerator _generator;
    [SerializeField] public CellMapController _cellMapController;
    [SerializeField] public RoomMapController _roomMapController;
    private readonly List<IMapController>_controllers = new();

    public static MazeController Instance { get;private set; }

    private void Awake()
    {
        if (Instance != null && Instance!=this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        _generator = new MazeGenerator();
        _roomMapController = GetComponentInChildren<RoomMapController>();
        _cellMapController= GetComponentInChildren<CellMapController>();
        _controllers.Add(_cellMapController);
        _controllers.Add(_roomMapController);
        _generator.Generator(Rows, Columns);
        SetCellData(_generator.Gird);
    }

    private void Start()
    {

    }
    private void SetCellData(Cell[] cellList)
    {
        foreach (var cell in cellList)
        {
            foreach(var controller in _controllers)
            {
                controller.AddCell(cell);
            }
        }
    }

    public CellControll GetNextCell(CellControll cellControll, Vector2 direction)
    {
        direction = cellControll.GetGridPosition() + direction;
        int indexe = (int)direction.y * -this.Columns + (int)direction.x;
        return _cellMapController.GetValue(indexe);
    }

    public CellControll GetStartCell()
    {
        var start = _cellMapController.GetValue(0);
        return start;
    }

    public RoomController GetNextRoom(RoomController roomController, Vector2 direction)
    {
        direction = roomController.GetGridPosition() + direction;
        int indexe = (int)direction.y * -this.Columns + (int)direction.x;
        return _roomMapController.GetValue(indexe);
    }

    public RoomController GetStartRoom()
    {
        var start = _roomMapController.GetValue(0);
        return start;
    }

}
