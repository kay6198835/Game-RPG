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
        //Avatar.transform.position = current.transform.position;
        current = MazeController.Instance.GetStartCell();
    }

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, Move);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, Move);
    }

    public void Move(object ojt = null)
    {
        next = MazeController.Instance.GetNextCell(current,(Vector2)ojt);
    }
    public void OnMove()
    {
        current = next;
    }
}