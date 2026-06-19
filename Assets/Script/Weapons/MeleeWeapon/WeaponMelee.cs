using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class WeaponMelee : Weapon
{
    [Header("Melee Weapon")]
    [SerializeField] private WeaponMeleeStats statsMelee;
    private int currentStateIndex = 0;
    private int comboCount = 0;
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
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(centerAttackPosition, currrentSA.attackRange, statsMelee.LayerMask);
        int damage = Mathf.RoundToInt(currrentSA.attackDamege * currrentSA.damageMultiplier);
        bool didHit = false;
        foreach (Collider2D enemy in hitEnemies)
        {
            INegativeReceiver dmg = enemy.GetComponentInChildren<INegativeReceiver>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage, transform.position);
                didHit = true;
            }
        }
        if (didHit)
        {
            comboCount++;
            EventManager.Emit(EventID.ON_COMBO_HIT, comboCount);
        }
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
    {
        if (base.CheckCanAttack(player) && !inputHandler.BufferIsAttack)
        {

            if (currentStateIndex == statsMelee.AttackState.Count ||
            lastClickTime + deplayTime < Time.time)
            {
                if (lastClickTime + deplayTime < Time.time) comboCount = 0;
                currentStateIndex = 0;
            }
            SetAnimation(player);
        }
        else
        {
            Debug.Log("Can't call check Attack");
            canAttack = false;
        }
        return canAttack;
    }

    public override void ResetCombo()
    {
        currentStateIndex = 0;
        comboCount = 0;
    }

    public override void SetAnimation(Player player)
    {
        deplayTime = DurationNextAttack() * 0.8f;
        if (currentStateIndex > statsMelee.AttackState.Count) currentStateIndex = 0;
        currrentSA = statsMelee.AttackState[currentStateIndex];
        // Send AnimationController to Player by Event
        player.Anim.runtimeAnimatorController = currrentSA.directionAttackAnimatorOV;
        player.Anim.speed = 0.2f;
        //Attack Position
        CenterAttackPosition(player);
        currentStateIndex++;
    }
    private float DurationNextAttack()
    {
        //List<KeyValuePair<AnimationClip, AnimationClip>> overridesClip;

        //overridesClip = new List<KeyValuePair<AnimationClip, AnimationClip>>(statsMelee.AttackState[currentStateIndex].directionAttackAnimatorOV.overridesCount);

        //statsMelee.AttackState[currentStateIndex].directionAttackAnimatorOV.GetOverrides(overridesClip);

        //durationNextAttack = overridesClip.

        var clipPairs = statsMelee.AttackState[currentStateIndex].directionAttackAnimatorOV.clips;

        float totalDuration = 0f;

        foreach (var pair in clipPairs)
        {
            if (pair.overrideClip != null)
            {
                totalDuration += pair.overrideClip.length;
            }
        }
        Debug.Log("totalDuration" + totalDuration / 8);
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