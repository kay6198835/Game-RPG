using System;
using System.Collections;
using UnityEngine;
using VContainer;

public class SourceEffect : MonoBehaviour
{
    IObjectPoolService _objectPoolService;
    [Inject]
    public void Construct(IObjectPoolService objectPoolService)
    {
        _objectPoolService = objectPoolService;
    }
    public void SetSourceEffect(SourceEffectConfig context)
    {
        // Implementation for setting the source effect
        if (context.lifetime > 0)
        {
            StartCoroutine(OnEffectComplete(context.lifetime));
        }
    }

    IEnumerator OnEffectComplete(float duration)
    {
        yield return new WaitForSeconds(duration);
        // Implementation for when the effect is complete
        _objectPoolService.Despawn(gameObject);
    }
}

public class SourceEffectConfig
{
    public Action callback;
    public float lifetime;
}