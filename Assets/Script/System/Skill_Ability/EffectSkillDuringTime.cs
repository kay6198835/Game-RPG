using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Effect Ability/During Time")]
public class EffectSkillDuringTime : EffectSkillSO
{
    [SerializeField] public float amountIncreaseAmount;
    protected override float Effect(int dataIndex, float stats)
    {
        if (effectData[dataIndex].type.IsPercentage)
        {
            effectData[dataIndex].finalIncreaseAmount = stats * effectData[dataIndex].currentIncreaseAmount * Time.deltaTime;
        }
        else
        {
            effectData[dataIndex].finalIncreaseAmount = effectData[dataIndex].currentIncreaseAmount * Time.deltaTime;
        }
        stats += effectData[dataIndex].finalIncreaseAmount;
        amountIncreaseAmount += effectData[dataIndex].finalIncreaseAmount;
        return stats;
    }
    protected override float RemoveEffect(int dataIndex, float stats)
    {
        if (effectData[dataIndex].type.IsRecover)
        {
            stats -= amountIncreaseAmount;
        }
        effectData[dataIndex].finalIncreaseAmount = 0;
        amountIncreaseAmount = 0;
        return stats;
    }
}
