using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Effect Ability/One Time")]
public class EffectSkillOneTime : EffectSkillSO
{
    [SerializeField] private bool isActivate;
    public bool IsActivate { get => isActivate;}

    protected override void OnValidate()
    {
        base.OnValidate();
    }
    public override void DohEffect()
    {
        if (isActivate)
        {
            return;
        }
        base.DohEffect();
        isActivate = true;
    }
    public override void OffEffect()
    {
        base.OffEffect();
        isActivate = false;
    }
    protected override float Effect(int dataIndex, float stats)
    {
        if (effectData[dataIndex].type.IsPercentage)
        {
            effectData[dataIndex].amountIncreaseAmount = stats * effectData[dataIndex].currentIncreaseAmount;
        }
        else
        {
            effectData[dataIndex].amountIncreaseAmount = effectData[dataIndex].currentIncreaseAmount;
        }
        stats += effectData[dataIndex].amountIncreaseAmount;
        return stats;
    }
    protected override float RemoveEffect(int dataIndex, float stats)
    {
        stats -= effectData[dataIndex].amountIncreaseAmount;
        effectData[dataIndex].amountIncreaseAmount = 0;
        return stats;
    }
}
