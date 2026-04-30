using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Projectile
{
    [SerializeField] public EffectSkillSO effect;
    protected override void OnHitObject(RaycastHit2D hit)
    {
        IEffectable effectable = hit.collider.GetComponentInChildren<IEffectable>();
        if (effectable != null)
        {
            effectable.ApplyEffect(effect);
        }
        base.OnHitObject(hit);
    }



}
