using UnityEngine;

public class MapGridController : BaseGrid<MapCell>
{
    [SerializeField] GameObject Avatar;

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAP, OnLoadMap);
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, Move);
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnInitialLoad);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_LOAD_MAP, OnLoadMap);
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, Move);
        EventManager.UnResgister(EventID.ON_LOAD_MAZE_DONE, OnInitialLoad);
    }

    private void OnInitialLoad(object obj = null)
    {
        _current = GetStart();
        Avatar.transform.position = _current.transform.position;
    }

    private void OnLoadMap(object obj = null)
    {
        _current = _next;
        _next = null;
        Avatar.transform.position = _current.transform.position;
    }

    public void Move(object ojt = null)
    {
        _next = GetNext((Vector2)ojt);
    }
}
