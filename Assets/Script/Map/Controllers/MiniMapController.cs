using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
public class MiniMapController : MonoBehaviour
{
    [SerializeField] GameObject Avatar;
    [SerializeField] CellController current,next;
    private void Start()
    {

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

    public void Move(object ojt = null)
    {
        next = MazeController.Instance.CellMapController.GetNext((Vector2)ojt);
        LoadRoom();
    }
    public void OnMove()
    {
        current = next;
    }
    public void OnLoadMap(object ojt = null)
    {
        current = MazeController.Instance.CellMapController.GetStart();
    }

    public void LoadRoom()
    {
        current = next;
        next = null;
        EventManager.Emit(EventID.ON_LOAD_MAP);
    }
}