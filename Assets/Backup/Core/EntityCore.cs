using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCore : MonoBehaviour, IDamageable
{
    [SerializeField] private Entity entity;
    [SerializeField] private EntityMovement entityMovement;
    [SerializeField] private EntityFindTarget findTarget;
    [SerializeField] private EntityWeaponHolder weaponHolder;
    [SerializeField] private EntityEffectStats effectStats;
    #region Properties
    public Entity Entity { get => entity; }
    public EntityMovement EntityMovement { get => entityMovement;}
    public EntityFindTarget FindTarget { get => findTarget; }
    public EntityWeaponHolder WeaponHolder { get => weaponHolder; }
    public EntityEffectStats EffectStats { get => effectStats;}
    #endregion

    private void Update()
    {
        if (effectStats.Effect!= null) { effectStats.HandleEffect(); }
    }
    
    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        if (entity.Data.StatsSO.Health <= 0) return;
        entity.Data.StatsSO.ModifiersHealth -= amoutDamage;
        entity.Input.OnTakeDamage(attackPosition);
    }

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityMovement = GetComponentInChildren<EntityMovement>();
        findTarget = GetComponentInChildren<EntityFindTarget>();
        weaponHolder = GetComponentInChildren<EntityWeaponHolder>();
        effectStats = GetComponentInChildren<EntityEffectStats>();
    }
}
