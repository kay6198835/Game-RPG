using System;
using System.Collections;
using UnityEngine;

public class SpawnMono : MonoBehaviour, ISpawn
{
    protected float _duration;
    protected AbilityContext _context;
    protected Action<AbilityContext> _callback;
    public virtual void Launch(Vector2 target, float lifetime,
                             AbilityContext context, Action<AbilityContext> currentContext)
    {
        _duration = lifetime;
        _context = context;
        _callback = currentContext;
        if (_duration > 0)
        {
            StartCoroutine(DespawnOneselfAffterDuration());
        }

    }
    public virtual void DespawnOneSelf()
    {
        _context.Services.Pool.Release(gameObject);
    }

    protected virtual IEnumerator DespawnOneselfAffterDuration()
    {
        yield return new WaitForSeconds(_duration);
        _context.Services.Pool.Release(gameObject);
    }
}