using Unity.VisualScripting;
using UnityEngine;

public class MapTracker : MonoBehaviour
{
    [SerializeField] GameObject Avatar;
    [SerializeField] MapCell current, next;

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

    public void Move(object ojt = null)
    {
        next = MazeController.Instance.MapGrid.GetNext((Vector2)ojt);
        LoadRoom();
    }

    public void OnLoadMap(object ojt = null)
    {
        current = MazeController.Instance.MapGrid.GetStart();
        Avatar.transform.position = current.transform.position;
    }

    public void LoadRoom()
    {
        current = next;
        next = null;
        Avatar.transform.position = current.transform.position;
    }
}
