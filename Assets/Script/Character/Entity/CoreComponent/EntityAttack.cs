using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : EntityCoreComponent<EntityCore>
{
    private INegativeReceiver playerNegativeReceiver;
    private EntityInput entityInput;
    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityInput);
        playerNegativeReceiver = entityInput.TargetTransform.gameObject.GetComponent<INegativeReceiver>();
    }
    public void Attack()
    {
        playerNegativeReceiver.TakeDamage(10, Core.Entity.transform.position);
    }
}