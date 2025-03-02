using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
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

}

