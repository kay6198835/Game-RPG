using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static Dictionary<EventID, Action<object>> _events = new();

    public static void Resgister(EventID eventID, Action<object> action)
    {
        if (!_events.ContainsKey(eventID))
        {
            _events.Add(eventID, action);
        }
        else
        {
            _events[eventID] += action;
        }
    }

    public static void UnResgister(EventID eventID, Action<object> action)
    {
        if (_events.ContainsKey(eventID))
        {
            _events[eventID] -= action;
        }
    }

    public static void Emit(EventID eventID, object obj = null)
    {
        if (_events.ContainsKey(eventID))
        {
            _events[eventID]?.Invoke(obj);
        }
    }
}

public enum EventID
{
    ON_PLAYER_ON_DOOR,
    ON_PLAYER_DEATH,
    ON_REALOAD_GAME,
    ON_LOAD_MAZE_DONE,
    ON_LOAD_MAP,
    ON_CLEAR_ENEMY,
    ON_GET_SPAWN_POSITIONS,
    ON_DONE_SPAWN_ENEMY,
    ON_SPAWN_EXTRA_ENEMY,
    ON_TEST,
    ON_ENEMY_DEATH,
    ON_ROOM_CLEAR,
    ON_PLAYER_DEATH
}

