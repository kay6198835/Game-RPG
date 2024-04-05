using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbilitySO : ScriptableObject
{
    protected float starCastTime;
    protected float periodCastTime;
    protected float currentTime;
    public enum SkillType
    {
        DoCast,
        DoNonCast,
    }
    protected NewPlayer player;
    [Header("Stats Base")]
    [SerializeField] protected float maxCastTime;
    [SerializeField] protected float cooldownTime;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected AnimatorOverrideController animator;
    [SerializeField] private SkillType type;


    #region Attribute
    public string Name { get => name;}
    public float CooldownTime { get => cooldownTime;}
    public float TimeStarCast { get => starCastTime;}
    public float MaxCastTime { get => maxCastTime;}
    public float CurrentTime { get => currentTime;}
    public float PeriodCastTime { get => periodCastTime;}
    public AnimatorOverrideController Animator { get => animator; }
    public SkillType Type { get => type;}
    #endregion
    private void Awake()
    {
        layerMask = LayerMask.GetMask("Enemy");
    }
    public virtual void Enter(NewPlayer player)
    {
        this.player = player;
    }
    public virtual void Activate()
    {
        starCastTime = Time.time;
        currentTime = Time.time;
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
