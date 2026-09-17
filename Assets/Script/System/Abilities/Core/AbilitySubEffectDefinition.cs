using UnityEngine;

public abstract class AbilitySubEffectDefinition : ScriptableObject
{
    public abstract void ModifyPower(AbilityContext context, ref AbilityPower power);

    // AbilityInstance.Casting() pays the ability cost again for every effect that returns true,
    // so a sub-effect that runs while the button is held must be able to veto that.
    public virtual bool AllowCostTick(AbilityContext context) => true;
}
