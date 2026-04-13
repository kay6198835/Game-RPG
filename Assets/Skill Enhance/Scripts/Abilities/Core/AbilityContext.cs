using UnityEngine;

public class AbilityContext
{
    public IAbilityOwner Caster;
    public Vector3 Origin;
    public Vector3 Forward;
    public Vector3 TargetPoint;

    public float HoldTime;
    public float HoldRatio;

    public AbilityInstance AbilityInstance;
    public AbilityDefinition AbilityDefinition;
}
