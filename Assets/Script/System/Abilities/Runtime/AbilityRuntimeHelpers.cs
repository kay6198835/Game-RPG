using UnityEngine;

public static class AbilityRuntimeHelpers
{
    public static float SafeRatio(float current, float max)
    {
        if (max <= 0f) return 0f;
        return Mathf.Clamp01(current / max);
    }
}
