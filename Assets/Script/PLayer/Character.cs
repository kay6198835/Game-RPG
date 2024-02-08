using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
[RequireComponent(typeof(CharacterAnimationManager))]
[RequireComponent(typeof(Rigidbody2D))] 
[RequireComponent(typeof(Animator))] 
public class Character : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] protected bool isFreeze;
    [SerializeField] public bool isDeath;
    [SerializeField] protected float frezeeTime;
    [SerializeField] protected int currentHealth;

    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D rigidbodyCharacter;
    [SerializeField] protected CharacterAnimationManager animationManager;
    [SerializeField] protected StatsCharacter stats;
    [SerializeField] protected bool isAbility;

    [Header("Knock Back")]
    [SerializeField] protected bool isKnockBack;
    [SerializeField] protected float knockBackForce;
    [SerializeField] protected float knockBackDuration;

    [Header("Stun")]
    [SerializeField] protected bool isStun;

    [SerializeField] protected EventHandler OnIsFreezeChange;
    public bool IsFreeze
    {
        get => isFreeze;
        set
        {
            if (isFreeze != value)
            {
                isFreeze = value;
                OnIsFreezeChange?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public Rigidbody2D RigidbodyCharacter { get => rigidbodyCharacter;}
    public Animator Animator { get => animator; }
    public CharacterAnimationManager AnimationManager { get => animationManager;}
    public bool IsAbility { get => isAbility; set => isAbility = value; }
    public bool IsDeath { get => isDeath;}

    //public StatsCharacter Stats { get => stats; set => stats = value; }


    protected virtual void SettingCharacter()
    {
        OnIsFreezeChange += CheckIsStun;
        //OnIsFrezeeChange += CheckIsDeath;
        currentHealth = stats.maxHealth;
    }
    public void IsOnAbility()
    {
        isAbility = true;
    }
    public void IsOffAbility()
    {
        isAbility = false;
    }
    void CheckIsStun(object sender, EventArgs e)
    {
        if (isStun == true)
        {
            isFreeze = true;
        }
    }
    void CheckIsDeath(object sender, EventArgs e)
    {
        if (isDeath == true)
        {
            isFreeze = true;
        }
    }
    protected virtual void LoadCharacter()
    {
        animator = GetComponent<Animator>();
        rigidbodyCharacter = GetComponent<Rigidbody2D>();
        animationManager = GetComponent<CharacterAnimationManager>();
    }
    private void Update()
    {
        
    }
    public virtual void TakeDamage(int damage, GameObject caller)
    {
        if (damage > 0)
        {
            animationManager.Animation_3_Hit();
            KnockBack(caller, knockBackForce);

            currentHealth -= damage;
        }
        if (currentHealth <= 0)
        {
            rigidbodyCharacter.bodyType = RigidbodyType2D.Static;
            isDeath = true;
            animationManager.Animation_4_Death();
        }
    }
    public virtual void KnockBack(GameObject caller, float knockBackForce)
    {
        FrezeeRigidbody(frezeeTime);
        Vector2 direction = this.transform.position - caller.transform.position;
        direction = direction.normalized;
        rigidbodyCharacter.AddForce(direction * knockBackForce);
    }
    protected virtual void KnockBack1(GameObject caller, float knockBackForce)
    {
        isKnockBack = true;
        rigidbodyCharacter.velocity = (this.transform.position - caller.transform.position).normalized * knockBackForce;
        Debug.Log("Start KnockBack" + Time.time);
        StartCoroutine(CheckKnockBack1(knockBackDuration));
    }

    protected IEnumerator CheckKnockBack1(float knockBackDuration)
    {
        yield return new WaitForSeconds(knockBackDuration);
        if (isKnockBack)
        {
            Debug.Log("End KnockBack"+Time.time);
            isKnockBack = false;
            rigidbodyCharacter.velocity = Vector2.zero;
        }
    }

    public virtual void FrezeeRigidbody(float timeFrezee)
    {
        if (!isFreeze)
        {
            RigidbodyType2D origanal = rigidbodyCharacter.bodyType;
            rigidbodyCharacter.bodyType = RigidbodyType2D.Dynamic;
            isFreeze = true;
            StartCoroutine(WaitFrezee(timeFrezee,origanal));
        }
    }

    protected virtual IEnumerator WaitFrezee(float timeFrezee, RigidbodyType2D origanal)
    {
        yield return new WaitForSeconds(timeFrezee);
        rigidbodyCharacter.bodyType = origanal;
        IsFreeze = false;
    }
    public void Stun(float timeStun)
    {
        isStun = true;
        isFreeze = true;
        StartCoroutine(WaitStun(timeStun));
    }
    protected virtual IEnumerator WaitStun(float timeStun)
    {
        yield return new WaitForSeconds(timeStun);
        isStun = false;
        isFreeze = false;
    }
    protected virtual void Die()
    {
        Debug.Log(name + " Death");
        this.gameObject.SetActive(false);
    }
}