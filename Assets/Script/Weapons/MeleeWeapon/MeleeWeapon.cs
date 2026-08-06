using UnityEngine;
public class MeleeWeapon : Weapon
{
    public int currentStateIndex { get; private set; } = 0;
    private Vector2 centerAttackPosition;
    private AttackSO currrentSA;
    protected PlayerInputHandler inputHandler;
    protected MeleeWeaponStats StatsMelee => (MeleeWeaponStats)stats;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {

    }
    public override void Attack()
    {

    }
    public void MakeDamage()
    {
        // if (other.TryGetComponent(out INegativeReceiver receiver))
        //     receiver.TakeDamage(10, Core.Entity.transform.position);
    }
    public override void SetAbility()
    {
        // if (inputHandler.Skill == PlayerInputHandler.SkillType.Ability)
        // {
        //     currentAbilitySO = statsMelee.AbilityWeapon;
        // }
        // else if (inputHandler.Skill == PlayerInputHandler.SkillType.Special)
        // {
        //     currentAbilitySO = statsMelee.SkillWeapon;
        // }
        base.SetAbility();
    }
    public override void Equid(WeaponHolder weaponHolder)
    {
        base.Equid(weaponHolder);
        weaponHolder.Core.GetCoreComponent(out inputHandler);
    }
    public override bool CheckCanAttack()
    {                                                                                  // dòng 52 (anchor)
        if (base.CheckCanAttack() && !inputHandler.BufferIsAttack)
        {
            //SetAnimation();
        }
        else
        {
            canAttack = false;
        }
        return canAttack;
    }

    public override void SetAnimation(Player player)
    {
        if (currentStateIndex >= StatsMelee.AttackState.Count
        //|| lastClickTime + deplayTime < Time.time
        )
        {
            currentStateIndex = 0;
        }
        lastClickTime = Time.time;
        player.Anim.speed = 1f;
        var overrides = Utility.GetOverrideClips(
            StatsMelee.AttackState[currentStateIndex].directionAttackAnimatorOV, "Attack");
        deplayTime = Utility.DurationNextAttack(overrides)
        / player.Anim.speed;
        currrentSA = StatsMelee.AttackState[currentStateIndex];
        // Send AnimationController to Player by Event
        player.Anim.runtimeAnimatorController = currrentSA.directionAttackAnimatorOV;
        //Attack Position
        CenterAttackPosition(player);
        currentStateIndex++;
        lastClickTime = Time.time;
    }

    protected void CenterAttackPosition(Player player)
    {
        // need check late
        centerAttackPosition = (Vector2)player.transform.position
        + inputHandler.DirectionMouseVector.normalized * currrentSA.attackRange;
    }
    private void OnDrawGizmosSelected()
    {
        if (currrentSA != null)
        {
            Gizmos.DrawWireSphere(centerAttackPosition, currrentSA.attackRange);
        }
    }
}