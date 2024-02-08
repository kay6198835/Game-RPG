using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int Width, Height, X, Y;

    private bool updatedDoors = false;

    public Room(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Door leftDoor, rightDoor, botDoor, topDoor;

    public List<Door> doors = new List<Door>();

    void Start()
    {
        if(RoomController.instance == null)
        {
            Debug.Log("You in the wrong scene!");
            return;
        }

        Door[] ds = GetComponentsInChildren<Door>();

        foreach(Door d in ds)
        {
            doors.Add(d);
            switch (d.doorType)
            {
                case Door.DoorType.right: rightDoor = d; break;
                case Door.DoorType.left: leftDoor = d; break;
                case Door.DoorType.bottom: botDoor = d; break;
                case Door.DoorType.top: topDoor = d; break;
            }
        }

        RoomController.instance.RegisterRoom(this); 
    }

    private void Update()
    {
        if(name.Contains("End") && !updatedDoors)
        {
            RemoveUnconnectedDoors();
            updatedDoors = true;
        }
    }

    public void RemoveUnconnectedDoors()
    {
        foreach(Door door in doors)
        {
            switch(door.doorType)
            {
                case Door.DoorType.right: if(GetRight() == null) door.gameObject.SetActive(false); break;
                case Door.DoorType.left: if (GetLeft() == null) door.gameObject.SetActive(false); break;
                case Door.DoorType.bottom: if (GetBottom() == null) door.gameObject.SetActive(false); break;
                case Door.DoorType.top: if (GetTop() == null) door.gameObject.SetActive(false); break;
            }
        }
    }

    public Room GetRight()
    {
        if (RoomController.instance.DoesRoomExit(X + 1, Y))
            return RoomController.instance.FindRoom(X + 1, Y);
        else return null; 
    }

    public Room GetLeft()
    {
        if (RoomController.instance.DoesRoomExit(X - 1, Y))
            return RoomController.instance.FindRoom(X - 1, Y);
        else return null;
    }

    public Room GetTop()
    {
        if (RoomController.instance.DoesRoomExit(X, Y + 1))
            return RoomController.instance.FindRoom(X, Y + 1);
        else return null;
    }

    public Room GetBottom()
    {
        if (RoomController.instance.DoesRoomExit(X, Y - 1))
            return RoomController.instance.FindRoom(X, Y - 1);
        else return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0));
    }

    public Vector3 GetRoomCentre()
    {
        return new Vector3(X*Width, Y*Height);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            RoomController.instance.OnPlayerEnterRoom(this);
        }
    }
}
