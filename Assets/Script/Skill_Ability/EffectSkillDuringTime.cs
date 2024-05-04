using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Effect Ability/During Time")]
public class EffectSkillDuringTime : EffectSkillSO
{
    [SerializeField] public GameObject particle;
    protected override float Effect(int dataIndex, float stats)
    {
        return stats;
    }

    protected override float RemoveEffect(int dataIndex, float stats)
    {
        return stats;
    }
}
