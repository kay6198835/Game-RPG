using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class WeaponMelee : Weapon
{
    [Header("Melee Weapon")]
    [SerializeField] private WeaponMeleeStats statsMelee;
    private int currentStateIndex = 0;
    private Vector2 centerAttackPosition;
    private AttackSO currrentSA;
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
        deplayTime = 0.5f;
        durationNextAttack = 2f;
    }
    public override void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(centerAttackPosition, currrentSA.attackRange, statsMelee.LayerMask);
        foreach (Collider2D enemy in hitEnemies)
        {

        }
    }
    public override void SetAbility()
    {
        if (holder.Core.Player.StatsBehavior.Skill == PlayerInputHandler.SkillType.Ability)
        {
            currentAbilitySO = statsMelee.AbilityWeapon;
        }
        else if(holder.Core.Player.StatsBehavior.Skill == PlayerInputHandler.SkillType.Special)
        {
            currentAbilitySO = statsMelee.SkillWeapon;
        }
        base.SetAbility();
    }
    public override bool CheckCanAttack(NewPlayer player)
    {
        if (base.CheckCanAttack(player))
        {

            if (currentStateIndex == statsMelee.AttackState.Count || lastClickTime + durationNextAttack + deplayTime < Time.time)
            {
                currentStateIndex = 0;
            }
            durationNextAttack = DurationNextAttack();
            currrentSA = statsMelee.AttackState[currentStateIndex];
            player.Anim.runtimeAnimatorController = currrentSA.directionAttackAnimatorOV;
            //Attack Position
            CenterAttackPosition(player);
            currentStateIndex++;
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
        return canAttack;
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
        Debug.Log("totalDuration" + totalDuration);
        return totalDuration/8;
    }

    protected void CenterAttackPosition(NewPlayer player)
    {
        centerAttackPosition = (Vector2)player.transform.position + player.StatsBehavior.DirectionMouseVector.normalized * currrentSA.attackRange;
    }
    private void OnDrawGizmosSelected()
    {
        if(currrentSA != null)
        {
            Gizmos.DrawWireSphere(centerAttackPosition, currrentSA.attackRange);
        }
    }
}