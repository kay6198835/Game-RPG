using System;
using UnityEngine;

public class SourceEffect : MonoBehaviour
{
    public void SetSourceEffect(SourceEffectConfig context)
    {
        // Implementation for setting the source effect
    }
}

public class SourceEffectConfig
{
    public Action callback;
    public float lifetime;
}