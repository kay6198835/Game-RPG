using UnityEngine;

public class MapGridController : BaseGrid<MapCell>
{
    [SerializeField] GameObject Avatar;
    public override void Setting(int columns, int rows)
    {
        base.Setting(columns, rows);
        {
            Vector2 position = new Vector2();
            position.y = MazeController.Instance.GetCellStart().Row;
            position.x = MazeController.Instance.GetCellStart().Column;
            int _startIndex = CaculateIndex(position);
            _current = GetValue(_startIndex);
        }
    }
    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, Move);
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnLoadMap);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, Move);
        EventManager.UnResgister(EventID.ON_LOAD_MAZE_DONE, OnLoadMap);
    }

    private void OnLoadMap(object obj = null)
    {
        _current.VisitRoom();
        //Avatar.transform.position = _current.transform.position;
        //Use dotween to scale up avatar in map
    }

    public void Move(object ojt = null)
    {
        _next = GetNext(directionToNextMap);
        _next.VisitRoom();
        //use dotween to move to _next's position;
        _current = _next;
        _next = null;
    }
}
