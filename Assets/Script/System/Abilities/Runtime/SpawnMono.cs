using System;
using System.Collections;
using UnityEngine;

public class SpawnMono : MonoBehaviour, ISpawn
{
    protected float _duration;
    protected AbilityContext _context;
    protected Action<object[]> _callback;
    public virtual void Launch(Vector2 target, float lifetime,
                             AbilityContext context)
    {
        _duration = lifetime;
        _context = context;
        _callback = execute;
        if (_duration > 0)
        {
            StartCoroutine(DespawnOneselfAffterDuration());
        }

    }
    protected virtual void DespawnOneSelf()
    {
        _pool.Release(gameObject);
    }

    protected virtual IEnumerator DespawnOneselfAffterDuration()
    {
        yield return new WaitForSeconds(_duration);
        _pool.Release(gameObject);
    }
}