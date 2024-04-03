using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCore : MonoBehaviour, IDamageable
{
    [SerializeField] private Entity entity;
    [SerializeField] private EntityMovement entityMovement;
    [SerializeField] private EntityFindTarget findTarget;
    public EntityMovement EntityMovement { get => entityMovement;}
    public EntityFindTarget FindTarget { get => findTarget; }
    public Entity Entity { get => entity;}

    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        if (entity.Data.currentHealth <= 0) return;
        entity.Data.currentHealth -= amoutDamage;
        entity.Input.OnTakeDamage(attackPosition);
    }

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityMovement = GetComponentInChildren<EntityMovement>();
        findTarget = GetComponentInChildren<EntityFindTarget>();
    }
}
