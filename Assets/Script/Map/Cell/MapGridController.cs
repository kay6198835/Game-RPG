using UnityEngine;

public class MapGridController : BaseGrid<MapCell>
{
    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAP, OnLoadMap);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_LOAD_MAP, OnLoadMap);
    }

    private void OnLoadMap(object obj = null)
    {
        _current = _next;
        _next = null;
    }
}
