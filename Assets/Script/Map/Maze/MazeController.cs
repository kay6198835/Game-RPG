using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{
    [SerializeField] public int Rows = 3;
    [SerializeField] public int Columns = 3;
    [SerializeField] MazeGenerator _generator;
    [SerializeField] public MapGridController MapGrid;
    [SerializeField] public RoomGridController RoomGrid;
    private readonly List<IGrid> _controllers = new();

    public static MazeController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        _generator = new MazeGenerator();
        RoomGrid = GetComponentInChildren<RoomGridController>();
        MapGrid = GetComponentInChildren<MapGridController>();
        _controllers.Add(MapGrid);
        _controllers.Add(RoomGrid);
        _generator.Generator(Rows, Columns);
    }

    private void Start()
    {
        SetCellData(_generator.Gird);
    }

    private void SetCellData(Cell[] cellList)
    {
        foreach (var cell in cellList)
            foreach (var controller in _controllers)
                controller.AddCell(cell);

        foreach (var controller in _controllers)
            controller.Setting(Columns, Rows);

        EventManager.Emit(EventID.ON_LOAD_MAZE_DONE);
    }
}
