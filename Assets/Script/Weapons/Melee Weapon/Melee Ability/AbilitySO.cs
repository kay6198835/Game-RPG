using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbilitySO : ScriptableObject
{
    [Header("Stats Base")]
    [SerializeField] protected new string name;
    [SerializeField] protected float cooldownTime;
    [SerializeField] protected float activeTime;
    [SerializeField] protected float timeStarCast;
    [SerializeField] protected float maxCastTime;
    [SerializeField] protected float currentTime;
    [SerializeField] protected float periodCastTime;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected Player playerClone;
    [SerializeField] protected List<AnimatorOverrideController> animators;
    #region Attribute
    public string Name { get => name;}
    public float CooldownTime { get => cooldownTime;}
    public float ActiveTime { get => activeTime;}
    public float TimeStarCast { get => timeStarCast;}
    public float MaxCastTime { get => maxCastTime;}
    public float CurrentTime { get => currentTime;}
    public float PeriodCastTime { get => periodCastTime;}
    #endregion
    private void Awake()
    {
        layerMask = LayerMask.GetMask("Enemy");
    }
    public virtual void Activate(GameObject player)
    {
        timeStarCast = Time.time;
        currentTime = Time.time;
        playerClone = player.GetComponent<Player>();
    }
    public virtual void CastSkill(GameObject player)
    {
        currentTime = Time.time;
    }
    public virtual void BeginCooldown(GameObject player)
    {
        if (currentTime - timeStarCast < maxCastTime)
        {
            periodCastTime = currentTime - timeStarCast;
        }
        else
        {
            periodCastTime = maxCastTime;
        }
    }
}
