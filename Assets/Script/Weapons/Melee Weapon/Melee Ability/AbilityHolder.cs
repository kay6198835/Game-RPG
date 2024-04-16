using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityHolder : CoreCompoment
{
    [SerializeField] private AbilitySO ability;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float activeTime;
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private bool canUseAbility;
    [SerializeField] private int stateIndex;
    [SerializeField] private int stateLength;
    public enum SkillState
    {
        Start,
        Cast,
        Do,
        Exit
    }
    [SerializeField] private SkillState currentState;
    public AbilitySO Ability { get => ability; }
    public bool CanUseAbility { get => canUseAbility; }

    protected override void Awake()
    {
        base.Awake();
        stateIndex = 1;
        stateLength = Enum.GetNames(typeof(SkillState)).Length;
    }
    private void Start()
    {
        canUseAbility = false;
    }
    public void SetCanUseAbility(bool canUseAbility)
    {
        if (this.canUseAbility == canUseAbility)
        {
            return;
        }
        this.canUseAbility = canUseAbility;
        core.Player.Anim.SetBool("DoAB", !this.canUseAbility);
    }
    public void SetAblityWeapon(AbilitySO ability)
    {
        this.ability = ability;
    }
    public void EnterAbility()
    {
        core.Player.Anim.runtimeAnimatorController = ability.Animator;
        ability.Enter(core.Player);
        currentState = SkillState.Start;
    }
    public void ExitAbility()
    {
        ability.Exit();

    }
    public void SetStateAbility()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case SkillState.Start:
                ability.Activate();
                currentState = SkillState.Cast;
                break;
            case SkillState.Cast:
                ability.Cast();
                if (core.Player.InputHandler.State == PlayerInputHandler.SkillState.Do || ability.Type == AbilitySO.SkillType.DoNonCast)
                {
                    SetCanUseAbility(false);
                    currentState = SkillState.Do;
                }
                break;
            case SkillState.Do:
                ability.Do();
                currentState = SkillState.Exit;
                break;
        }
    }
}