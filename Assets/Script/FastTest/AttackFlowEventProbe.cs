using System;
using UnityEngine;

// Added at runtime by AttackFlowDebugger to the GameObject that owns the player Animator. Unity delivers
// an Animation Event to every component on that GameObject that has a method of the event's name, so
// this logs each attack event next to Player's own handler without touching Player. Delete when done.
public class AttackFlowEventProbe : MonoBehaviour
{
    public Action<string, AnimationEvent> OnEvent;

    private void AnimationStart(AnimationEvent e) => OnEvent?.Invoke(nameof(AnimationStart), e);
    private void AnimationTrigger(AnimationEvent e) => OnEvent?.Invoke(nameof(AnimationTrigger), e);
    private void AnimationOnAction(AnimationEvent e) => OnEvent?.Invoke(nameof(AnimationOnAction), e);
    private void AnimationOffAction(AnimationEvent e) => OnEvent?.Invoke(nameof(AnimationOffAction), e);
    private void AnimationFinishTrigger(AnimationEvent e) => OnEvent?.Invoke(nameof(AnimationFinishTrigger), e);
    private void AnimationEnd(AnimationEvent e) => OnEvent?.Invoke(nameof(AnimationEnd), e);
}
