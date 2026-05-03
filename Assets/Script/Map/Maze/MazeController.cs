using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MazeController : MonoBehaviour
{
    [SerializeField] public int Rows = 3;
    [SerializeField] public int Columns = 3;
    [SerializeField] MazeGenerator _generator;
    [SerializeField] public CellMapController CellMapController;
    [SerializeField] public RoomMapController RoomMapController;
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
        RoomMapController = GetComponentInChildren<RoomMapController>();
        CellMapController= GetComponentInChildren<CellMapController>();
        _controllers.Add(CellMapController);
        _controllers.Add(RoomMapController);
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
                controller.Setting(Columns,Rows);
            }
        }
    }

    public CellController GetNextCell(CellController cellControll, Vector2 direction)
    {
        direction = cellControll.GetGridPosition() + direction;
        int indexe = (int)direction.y * this.Columns + (int)direction.x;
        return CellMapController.GetValue(indexe);
    }

    public CellController GetStartCell()
    {
        var start = CellMapController.GetValue(0);
        return start;
    }

}
