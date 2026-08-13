using UnityEngine;

public class EntityWeaponMelee : EntityWeapon
{
    [Header("Melee Weapon")]
    [SerializeField] private MeleeWeaponStats statsMelee;
    private int currentStateIndex = 0;
    private Vector2 centerAttackPosition;
    private AttackSO currrentSA;
    private IAimProvider aim;
    private EntityInput entityInput;
    protected override void Awake()
    {
        base.Awake();
        statsMelee = stats as MeleeWeaponStats;
        if (holder != null)
        {
            holder.Core.GetCoreComponent(out entityInput);
            aim = entityInput;
        }
    }
    private void Start()
    {
        deplayTime = 0.5f;
        durationNextAttack = 0.9f;
    }
    public override void Attack()
    {
        Collider2D hitEPlayer = Physics2D.OverlapCircle(centerAttackPosition, currrentSA.attackRange, statsMelee.LayerMask);
        if (hitEPlayer != null)
        {
            INegativeReceiver damageable = hitEPlayer.GetComponentInChildren<INegativeReceiver>();
            if (damageable != null)
            {
                damageable.TakeDamage(currrentSA.attackDamege, transform.position);
            }
        }

    }
    public override ActivateSkill SetAbility()
    {
        if (entityInput.Skill == EntityInput.SkillType.Ability)
        {
            currentAbilitySO = statsMelee.AbilityWeapon;
        }
        else if (entityInput.Skill == EntityInput.SkillType.Special)
        {
            currentAbilitySO = statsMelee.SkillWeapon;
        }
        return currentAbilitySO;
    }
    public override bool CheckCanAttack(Entity entity, float lastClickTime)
    {
        if (base.CheckCanAttack(entity, lastClickTime))
        {
            if (currentStateIndex >= statsMelee.StageCount || lastClickTime + durationNextAttack + deplayTime < Time.time)
            {
                currentStateIndex = 0;
            }
            currrentSA = statsMelee.GetStage(currentStateIndex);
            entity.Anim.runtimeAnimatorController = currrentSA.directionAttackAnimatorOV;
            CenterAttackPosition(entity);
            currentStateIndex++;
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
        return canAttack;
    }
    protected void CenterAttackPosition(Entity entity)
    {
        centerAttackPosition = (Vector2)entity.transform.position + aim.AimDirection.normalized * currrentSA.attackRange;
    }
    private void OnDrawGizmosSelected()
    {
        if (currrentSA != null)
        {
            Gizmos.DrawWireSphere(centerAttackPosition, currrentSA.attackRange);
        }
    }
}
