using UnityEngine;
using DG.Tweening;

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
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, Move);
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnLoadMap);
    }

    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, Move);
        EventManager.UnResgister(EventID.ON_LOAD_MAZE_DONE, OnLoadMap);
    }

    public void OnLoadMap(object obj = null)
    {
        _current.VisitRoom();
        Avatar.transform.position = _current.transform.position;
        Avatar.transform.localScale = Vector3.zero;
        Avatar.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void Move(object obj = null)
    {
        _next = GetNext((Vector2)obj);
        _next.VisitRoom();
        Avatar.transform.DOMove(_next.transform.position, 0.3f).SetEase(Ease.InOutQuad);
        _current = _next;
        _next = null;
    }
}
