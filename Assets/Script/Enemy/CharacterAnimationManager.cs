using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator characterAnimation;
    [SerializeField] private Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
        characterAnimation = character.Animator;
    }
    public void Animation_1_Idle()
    {
        characterAnimation.SetBool("Run", false);
    }
    public void Animation_2_Run()
    {
        characterAnimation.SetBool("Run", true);
    }
    public void Animation_3_Hit()
    {
        characterAnimation.SetBool("Run", false);
        characterAnimation.SetTrigger("Hit");
    }
    public void Animation_4_Death()
    {
        characterAnimation.SetBool("Run", false);
        characterAnimation.SetTrigger("Death");
    }
    public void Animation_5_Ability()
    {
        characterAnimation.SetBool("Run", false);
        characterAnimation.SetBool("Ability", true);
    }
    public void Animation_5_OfAbility()
    {
        characterAnimation.SetBool("Ability", false);
    }
    public void Animation_5_Ability2()
    {
        characterAnimation.SetBool("Run", false);
        characterAnimation.SetBool("Ability 2", true);
    }
    public void Animation_5_Ability3()
    {
        characterAnimation.SetBool("Run", false);
        characterAnimation.SetBool("Ability 3", true);
    }
    public void Animation_6_Attack()
    {
        characterAnimation.SetBool("Run", false);
        characterAnimation.SetTrigger("Attack");
    }

    public void Animation_7_Attack2()
    {
        characterAnimation.SetBool("Run", false);
        characterAnimation.SetTrigger("Attack 2");
    }
    public void Animation_8_Attack3()
    {
        characterAnimation.SetBool("Run", false);
        characterAnimation.SetTrigger("Attack 3");
    }
    public void Animation_Direction(int direction)
    {
        characterAnimation.SetFloat("Direction", direction);
    }

}
