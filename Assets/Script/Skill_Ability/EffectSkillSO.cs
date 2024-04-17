using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Effect Ability")]
public class EffectSkillSO : AbstractSkillSO
{
    [SerializeField] private List<EffectData> effectData;
    [SerializeField] private EntityData entityData;
    [SerializeField] private float lifeTime;
    [SerializeField] private float validityDuration;

    public List<EffectData> EffectData { get => effectData; }
    public float LifeTime { get => lifeTime;}
    public float ValidityDuration { get => validityDuration; }

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
    public void OnEffect(EntityData entityData)
    {
        this.entityData = entityData;
    }
    public void DoinghEffect()
    {
        for (int i = 0; i < effectData.Count; i++)
        {
            if (effectData[i].type.IsOTA && effectData[i].isOnActivate)
            {
                return;
            }
            else
            {
                switch (EffectData[i].statsTypes)
                {
                    case Stats.Health:
                        Effect(i,ref entityData.currentHealth);
                        break;
                    case Stats.SpeedMove:
                        Effect(i, ref entityData.currentVelocities);
                        break;
                    case Stats.Amor:
                        Effect(i, ref entityData.amor);
                        break;
                }
                effectData[i].isOnActivate = true;
            }
        }
    }
    protected virtual void Effect(int dataIndex, ref float stats)
    {
        if (effectData[dataIndex].type.IsPercentage)
        {
            stats += stats * effectData[dataIndex].currentIncreaseAmount;
        }
        else
        {
            stats +=stats;
        }
    }
    public void OffEffect()
    {
        for (int i = 0; i < effectData.Count; i++)
        {
            effectData[i].isOnActivate = false;
        }
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
    public bool isOnActivate;
    public float effectIncreaseAmount;
    public float currentIncreaseAmount;
}
[System.Serializable]
public class EffectType
{
    [SerializeField] private bool isOTA;
    [SerializeField] private bool isNegative;
    [SerializeField] private bool isPercentage;
    public bool IsOTA { get => isOTA;}
    public bool IsNegative { get => isNegative;}
    public bool IsPercentage { get => isPercentage; }
}
public enum Stats
{
    Health,
    SpeedMove,
    Amor,
}



