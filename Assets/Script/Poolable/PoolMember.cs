using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PoolMember : MonoBehaviour
{
    [SerializeField] private Pool pool;
    [SerializeField] public bool isInPool { get; private set; } = false;
    public void Initialize(Pool pool)
    {
        this.pool = pool;
    }

    public Pool GetPool()
    {
        return pool;
    }

    public void SwitchIsInPool(bool SwitchInPool)
    {
        isInPool = SwitchInPool;
    }

}