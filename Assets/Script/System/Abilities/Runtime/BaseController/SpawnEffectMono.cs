using System;
using System.Collections;
using UnityEngine;

public class SpawnEffectMono : MonoBehaviour, ISpawnEffect
{
    protected float _duration;
    protected AbilityContext _context;
    protected Action<AbilityContext> _callback;
    public virtual void Launch(float lifetime,
                             AbilityContext context, Action<AbilityContext> callback)
    {
        _duration = lifetime;
        _context = context;
        _callback = callback;
        if (_duration > 0)
        {
            StartCoroutine(DespawnOneselfAffterDuration());
        }
    }
    public virtual void DespawnOneSelf()
    {
        _context.Services.Pool.Despawn(gameObject);
    }

    protected virtual IEnumerator DespawnOneselfAffterDuration()
    {
        yield return new WaitForSeconds(_duration);
        _context.Services.Pool.Despawn(gameObject);
    }


}