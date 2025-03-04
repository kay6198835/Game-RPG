using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    public static Dictionary<AnimationEventId, Action<object>> _events = new();
    public static void Resgister(AnimationEventId eventID, Action<object> action)
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

    public static void UnResgister(AnimationEventId eventID, Action<object> action)
    {
        if (_events.ContainsKey(eventID))
        {
            _events[eventID] -= action;
        }
    }

    public static void Emit(AnimationEventId eventID, object obj = null)
    {
        if (_events.ContainsKey(eventID))
        {
            _events[eventID]?.Invoke(obj);
        }
    }
}
public enum AnimationEventId
{
    StartAnimation,
    MoveAnimation,
    AttactAnimation,
    DoSkillAnimation,
    EndAnimation
}
