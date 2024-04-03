using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName= "Ability SO/Dash Ability")]
public class DashAbility : AbilitySO
{
    [SerializeField] private float dashingPower;

    public override void Activate()
    {
        base.Activate();
    }

    public override void CastSkill()
    {
        base.CastSkill();
    }

    public override void DoAbility()
    {
        base.DoAbility();
    }

    public override void ExitSkill()
    {
        base.ExitSkill();
    }
}