using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IEffectable
{
    public abstract void ApplyEffect(EffectSkillSO effect);
    public abstract void DoEffect();
    public abstract void HandleEffect();
    public abstract void RemoveEffect();
}
