using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffectStats : EntityCoreComponent,IEffectable
{
    //[SerializeField] private List<EffectSkillSO> effects;
    [SerializeField] private EffectSkillSO effect;
    [SerializeField] private float test;
    public EffectSkillSO Effect { get => effect; }
    //public List<EffectSkillSO> Effects { get => effects; }
    protected override void Awake()
    {
        base.Awake();
    }
    public void ApplyEffect(EffectSkillSO effect)
    {
        Debug.Log("Apply");
        this.effect = effect;
        this.effect.OnEffect(entityCore.Entity.Data.StatsSO);
    }

    public void HandleEffect()
    {
        if (!effect.IsDone)
        {
            DoEffect();
        }
        else
        {
            RemoveEffect();
        }
    }
    public void DoEffect()
    {
        this.effect.DohEffect();
        this.effect.ChecIsDone();
    }
    public void RemoveEffect()
    {
        effect.OffEffect();
        this.effect = null;
    }
}
