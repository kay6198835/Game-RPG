using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbilitySO : ScriptableObject
{
    [Header("Stats Base")]
    protected float starCastTime;
    [SerializeField] protected float maxCastTime;
    protected float periodCastTime;
    [SerializeField] protected float cooldownTime;
    protected float currentTime;
    [SerializeField] protected LayerMask layerMask;
    protected NewPlayer player;
    [SerializeField] protected AnimatorOverrideController animator;
    #region Attribute
    public string Name { get => name;}
    public float CooldownTime { get => cooldownTime;}
    //public float ActiveTime { get => activeTime;}
    public float TimeStarCast { get => starCastTime;}
    public float MaxCastTime { get => maxCastTime;}
    public float CurrentTime { get => currentTime;}
    public float PeriodCastTime { get => periodCastTime;}
    public AnimatorOverrideController Animator { get => animator; }
    #endregion
    private void Awake()
    {
        layerMask = LayerMask.GetMask("Enemy");
    }
    public virtual void Activate(NewPlayer player)
    {
        starCastTime = Time.time;
        currentTime = Time.time;
        this.player=player;
    }
    public virtual void CastSkill()
    {
        currentTime = Time.time;
    }
    public virtual void DoAbility()
    {
        if (currentTime - starCastTime < maxCastTime)
        {
            periodCastTime = currentTime - starCastTime;
        }
        else
        {
            periodCastTime = maxCastTime;
        }
    }
    public virtual void ExitSkill()
    {
        this.player = null;
    }
}
