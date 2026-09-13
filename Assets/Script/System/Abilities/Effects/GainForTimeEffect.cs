using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Gain For Time")]
public class GainForTimeEffect : AbilityEffectDefinition
{
    public float Duration = 5f;
    public float DamagePerTick = 25f;
    public float CurrentTickTime = 0f;
    public int TickCountMax = 5;
    public int CurrentTickCount = 0;
    public override void Apply(AbilityContext context)
    {
        if (CurrentTickCount >= TickCountMax)
        {
            Debug.LogWarning("[GainForTimeEffect] Đã đạt đến số lần tick tối đa.");
            return;
        }
        if (CurrentTickTime >= Duration)
        {
            CurrentTickCount++;
            CurrentTickTime = 0f;
        }
    }

    public void ReloadEffect()
    {
        CurrentTickCount = 0;
        CurrentTickTime = 0f;
    }
}