using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Play Debug Log")]
public class PlayDebugLogEffect : AbilityEffectDefinition
{
    [TextArea]
    public string Message;

    public override void Apply(AbilityContext context)
    {
        Debug.Log($"[Ability Debug] {Message} | HoldTime={context.HoldTime:F2} | HoldRatio={context.HoldRatio:F2}");
    }
}
