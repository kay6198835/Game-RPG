using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationPlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] public string movementAnimatorParameterName = "Movement";
    [SerializeField] public string idleAnimatorParameterName = "Idle";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        AnimationEventManager.Resgister(AnimationEventId.StartAnimation, StartAnimation);
        AnimationEventManager.Resgister(AnimationEventId.MoveAnimation, Move);
        AnimationEventManager.Resgister(AnimationEventId.AttactAnimation, Attack);
        AnimationEventManager.Resgister(AnimationEventId.DoSkillAnimation, DoSkill);
        AnimationEventManager.Resgister(AnimationEventId.StartAnimation, EndAnimation);
    }
    private void OnDisable()
    {
        AnimationEventManager.UnResgister(AnimationEventId.StartAnimation, StartAnimation);
        AnimationEventManager.UnResgister(AnimationEventId.MoveAnimation, Move);
        AnimationEventManager.UnResgister(AnimationEventId.AttactAnimation, Attack);
        AnimationEventManager.UnResgister(AnimationEventId.DoSkillAnimation, DoSkill);
        AnimationEventManager.UnResgister(AnimationEventId.StartAnimation, StartAnimation);
    }
    private void StartAnimation(object obj)
    {

    }
    private void EndAnimation(object obj)
    {

    }
    private void Move(object obj)
    {
        animator.Play(movementAnimatorParameterName);
    }
    private void Attack(object obj)
    {

    }
    private void DoSkill(object obj)
    {

    }
    void stes()
    {
        
    }
}
