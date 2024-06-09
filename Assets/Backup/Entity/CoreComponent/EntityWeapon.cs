using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public abstract class EntityWeapon : MonoBehaviour
{
    [SerializeField] protected EntityWeaponHolder holder;
    [SerializeField] protected WeaponStats stats;
    [SerializeField] protected ActivateSkill currentAbilitySO;
    protected float lastClickTime;
    protected float deplayTime;
    protected float durationNextAttack;
    protected bool canAttack;
    public ActivateSkill CurrentAbilitySO { get => currentAbilitySO; }
    protected float LastClickTime { get => lastClickTime;}
    protected float DeplayTime { get => deplayTime; }
    protected float DurationNextAttack { get => durationNextAttack;}
    protected bool CanAttack { get => canAttack;}
    protected virtual void Awake()
    {

    }
    public abstract void Attack();
    public virtual bool CheckCanAttack(Entity entity, float lastClickTime)
    {
        this.lastClickTime = lastClickTime;

        if (this.lastClickTime + deplayTime > Time.time)
        {
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
        return canAttack;
    }
    public abstract ActivateSkill SetAbility();
}
