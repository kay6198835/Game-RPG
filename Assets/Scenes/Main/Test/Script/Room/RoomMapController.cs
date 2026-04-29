using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMapController : MonoBehaviour, IMapController
{
    [SerializeField] RoomController prefabObject;
    [SerializeField] List<RoomController> _roomControlls;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
    public void AddCell(Cell cell)
    {
        RoomController roomControll = Instantiate(prefabObject, this.transform) as RoomController;
        roomControll.AddCell(cell);
        _roomControlls.Add(roomControll);
    }
    public RoomController GetValue(int index)
    {
        var roomControll = _roomControlls[index];
        return roomControll;
    }

    public void CheckValidateNextDoor(int index)
    {
        if (index < 0 || index >= _roomControlls.Count) return;
    }

    public void SetValue(int index, RoomController roomController)
    {
        _roomControlls[index] = roomController;
    }

    public Transform GetStartPosition(RoomController current,RoomController next)
    {
        return transform;
    }
    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public void Setting()
    {
        throw new System.NotImplementedException();
    }

    public void Setup()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
