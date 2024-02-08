//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Profiling;

//[CreateAssetMenu(menuName = "Ability SO/Block Ability")]
//public class BlockAbility : AbilitySO
//{
//    public override void Activate(GameObject player)
//    {
//        base.Activate(player);
//        playerClone.IsBlocking = true;
//        playerClone.Weapon.ShieldArea.isTrigger = false;
//    }
//    public override void CastSkill(GameObject player)
//    {
//        base.CastSkill(player);
//        playerClone.Animator.runtimeAnimatorController = animators[(int)playerClone.Animator.GetFloat("Direction")];
//        Debug.Log(playerClone.Animator.runtimeAnimatorController);
//        playerClone.GetComponent<Player>().AnimationManager.Animation_5_Ability();
//    }
//    public override void BeginCooldown(GameObject parent)
//    {
//        base.BeginCooldown(parent);
//        playerClone.IsBlocking = false;
//        playerClone.Weapon.ShieldArea.isTrigger = true;
//        playerClone.GetComponent<Player>().AnimationManager.Animation_5_OfAbility();
//    }
//}
