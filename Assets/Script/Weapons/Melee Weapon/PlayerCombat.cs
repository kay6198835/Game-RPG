using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : WeaponAttack
{
    [Range(0,1)]
    [SerializeField] protected float timeBetweenCombo;
    [SerializeField] protected float lastComboEnd;
    [SerializeField] protected int comboCounter;
    [SerializeField] protected bool endCombo=false;


    [SerializeField] protected WeaponMelee weaponMelee;
    [SerializeField] protected KeyCode keyCode;
    [SerializeField] protected Vector2 attackPosition;
    [SerializeField] protected AttackSO attackState;
    public AttackSO AttackState { get => attackState;}
    public Animator Animator { get => animator; set => animator = value; }
    public Player Player { get => player; set => player = value; }

    private void Awake()
    {
        //attackStates = weaponMelee.Stats.attackState;

    }
    private void Start()
    {
        //animator = GetComponentInParent<Animator>();
        attackState = weaponMelee.Stats.attackState[0];
    }
    //private void Update()
    //{
    //    if (player.IsDeath)
    //    {
    //        return;
    //    }
    //    RotationAttack();
    //    PositionAttack();
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        keyCode = KeyCode.Space;
    //        SpecicalAttack();
    //    }
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        keyCode = KeyCode.Mouse0;
    //        Combat();
    //    }
    //    else if (Input.GetMouseButtonDown(1))
    //    {
    //        keyCode = KeyCode.Mouse1;
    //        Auxiliary();
    //    }
    //    ExitAttack();
    //}
    protected override void DoAttack(Weapon currentweaponMelee)
    {
        if (Time.time - lastComboEnd > delayAttack*1.2)
        {
            CancelInvoke(nameof(EndCombo));
            endCombo = false;
            weaponMelee = (WeaponMelee)GetWeapon(currentweaponMelee);
            attackState = weaponMelee.Stats.attackState[comboCounter];
            if (attackState.directionAttackAnimatorOV != null)
            {
                animator.runtimeAnimatorController = attackState.
                    directionAttackAnimatorOV[(int)animator.GetFloat("Direction")];
                animator.Play("Attack", 0, 0);
            }
            if (attackState.ability != null)
            {
                weaponMelee.AttackAbility(Player.gameObject, keyCode);
            }
            MakeDamage();
            comboCounter++;
            lastClickedTime = Time.time;
            if (comboCounter + 1 > weaponMelee.Stats.attackState.Count)
            {
                EndCombo();
                endCombo = true;
            }
        }
        if (!endCombo)
        {
            ExitAttack();
            endCombo = true;
        }
    }
    void ExitAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            //Debug.Log("1");
        }
        Invoke(nameof(EndCombo), delayAttack * 1.3f);
    }
    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
    Weapon GetWeapon(Weapon currentweaponMelee)
    {
        if (currentweaponMelee != weaponMelee&& currentweaponMelee != null)
        {
            currentweaponMelee = weaponMelee;
        }
        return currentweaponMelee;
    }
    void MakeDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackState.attackRange / 2, attackState.enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackState.attackDamege, gameObject);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackState)
        {
            Gizmos.DrawWireSphere(attackPosition, attackState.attackRange / 2);
        }
    }
}
