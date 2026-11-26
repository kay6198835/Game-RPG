using System.Collections;
using UnityEngine;
using VContainer;

public class SpawnMono : MonoBehaviour, ISpawn
{
    IObjectPoolService objectPoolManager;
    [Inject]
    public void Construct(IObjectPoolService objectPoolManager)
    {
        this.objectPoolManager = objectPoolManager;
    }
    public void DespawnOneSelf()
    {
        objectPoolManager.Despawn(gameObject);
    }

    protected IEnumerator DespawnOneselfAffterDuration(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        DespawnOneSelf();
    }

    public void Launch(float lifetime)
    {
        if (lifetime > 0)
        {
            StartCoroutine(DespawnOneselfAffterDuration(lifetime));
        }
    }
}