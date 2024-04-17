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
        this.effect = effect;
        this.effect.OnEffect(entityCore.Entity.Data);
    }
    public void DoEffect()
    {

    }
    public void HandleEffect()
    {

        DoEffect();

    }
    public void RemoveEffect()
    {

    }
}
