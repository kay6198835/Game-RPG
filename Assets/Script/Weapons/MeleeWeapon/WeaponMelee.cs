using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class WeaponMelee : Weapon
{
    [Header("Melee Weapon")]
    [SerializeField] private WeaponMeleeStats statsMelee;
    public int currentStateIndex { get; private set; } = 0;
    private Vector2 centerAttackPosition;
    private AttackSO currrentSA;
    protected PlayerInputHandler inputHandler;
    protected override void Awake()
    {
        base.Awake();
        if (stats.GetType() == typeof(WeaponMeleeStats))
        {
            statsMelee = (WeaponMeleeStats)stats;

            //stats = new WeaponMeleeStats();
        }
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
        if (inputHandler.Skill == PlayerInputHandler.SkillType.Ability)
        {
            currentAbilitySO = statsMelee.AbilityWeapon;
        }
        else if (inputHandler.Skill == PlayerInputHandler.SkillType.Special)
        {
            currentAbilitySO = statsMelee.SkillWeapon;
        }
        base.SetAbility();
    }
    public override void Equid(WeaponHolder weaponHolder)
    {
        base.Equid(weaponHolder);
        weaponHolder.Core.GetCoreComponent(out inputHandler);
    }
    public override bool CheckCanAttack(Player player)
    {                                                                                  // dòng 52 (anchor)
        if (base.CheckCanAttack(player) && !inputHandler.BufferIsAttack)
        {
            SetAnimation(player);
        }
        else
        {
            canAttack = false;
        }
        return canAttack;
    }

    public override void SetAnimation(Player player)
    {
        if (currentStateIndex >= statsMelee.AttackState.Count
        //|| lastClickTime + deplayTime < Time.time
        )
        {
            currentStateIndex = 0;
        }
        lastClickTime = Time.time;
        player.Anim.speed = 1f;
        deplayTime = DurationNextAttack() / player.Anim.speed;
        currrentSA = statsMelee.AttackState[currentStateIndex];
        // Send AnimationController to Player by Event
        player.Anim.runtimeAnimatorController = currrentSA.directionAttackAnimatorOV;
        //Attack Position
        CenterAttackPosition(player);
        currentStateIndex++;
        lastClickTime = Time.time;
    }
    private float DurationNextAttack()
    {

        var clipPairs = statsMelee.AttackState[currentStateIndex].directionAttackAnimatorOV.clips;

        float totalDuration = 0f;

        foreach (var pair in clipPairs)
        {
            if (pair.overrideClip != null)
            {
                totalDuration += pair.overrideClip.length;
            }
        }
        return totalDuration / 8;
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