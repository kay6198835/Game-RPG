using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Effect Ability")]
public abstract class EffectSkillSO : AbstractSkillSO
{
    [SerializeField] protected List<EffectData> effectData;
    [SerializeField] protected EntityStatsSO statsSO;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected float validityDuration;
    [SerializeField] protected bool isDone;
    public List<EffectData> EffectData { get => effectData; }
    public float LifeTime { get => lifeTime; }
    public float ValidityDuration { get => validityDuration; }
    public bool IsDone { get => isDone; }

    protected override void OnValidate()
    {
        base.OnValidate();
        for (int i = 0; i < effectData.Count; i++)
        {
            if (!effectData[i].type.IsNegative)
            {
                effectData[i].currentIncreaseAmount = Math.Abs(effectData[i].effectIncreaseAmount);
            }
            else
            {
                effectData[i].currentIncreaseAmount = -Math.Abs(effectData[i].effectIncreaseAmount);
            }
            if (effectData[i].type.IsPercentage)
            {
                effectData[i].currentIncreaseAmount = effectData[i].currentIncreaseAmount / 100;
            }
        }
    }
    public virtual void OnEffect(EntityStatsSO statsSO)
    {
        this.statsSO = statsSO;
    }
    public virtual void DohEffect()
    {
        for (int i = 0; i < effectData.Count; i++)
        {
            switch (EffectData[i].statsTypes)
            {
                case Stats.Health:
                    statsSO.ModifiersHealth = Effect(i, statsSO.ModifiersHealth);
                    break;
                case Stats.SpeedMove:
                    statsSO.ModifiersVelocities = Effect(i, statsSO.ModifiersVelocities);
                    break;
                case Stats.Amor:
                    statsSO.ModifiersHealth = Effect(i, statsSO.ModifiersAmor);
                    break;
            }
        }
    }
    public virtual void OffEffect()
    {
        for (int i = 0; i < effectData.Count; i++)
        {
            switch (EffectData[i].statsTypes)
            {
                case Stats.Health:
                    statsSO.ModifiersHealth = RemoveEffect(i, statsSO.ModifiersHealth);
                    break;
                case Stats.SpeedMove:
                    statsSO.ModifiersVelocities = RemoveEffect(i, statsSO.ModifiersVelocities);
                    break;
                case Stats.Amor:
                    statsSO.ModifiersHealth = RemoveEffect(i, statsSO.ModifiersAmor);
                    break;
            }
        }
        validityDuration = 0;
        isDone = false;
        statsSO = null;
    }
    protected abstract float RemoveEffect(int dataIndex, float stats);
    protected abstract float Effect(int dataIndex, float stats);
    public virtual void ChecIsDone()
    {
        validityDuration += Time.deltaTime;
        isDone = (validityDuration >= lifeTime);
    }
    protected override void GenerateDescription()
    {

    }
}
[System.Serializable]
public class EffectData
{
    public EffectType type;
    public Stats statsTypes;
    public float effectIncreaseAmount;
    public float currentIncreaseAmount;
    public float finalIncreaseAmount;
    public GameObject particle;
}
[System.Serializable]
public class EffectType
{
    [SerializeField] private bool isNegative;
    [SerializeField] private bool isPercentage;
    [SerializeField] private bool isRecover;
    public bool IsNegative { get => isNegative; }
    public bool IsPercentage { get => isPercentage; }
    public bool IsRecover { get => isRecover; }
}
public enum Stats
{
    Health,
    SpeedMove,
    Amor,
}



