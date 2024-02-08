using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityHolder : MonoBehaviour
{
    public AbilitySO ability;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float activeTime;
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private Player playerClone;
    enum AbilityState
    {
        ready,
        active,
        cast,
        cooldown
    }
    [SerializeField] AbilityState state = AbilityState.ready;
    private void Update()
    {
        if (Input.GetKey(keyCode))
        {
            CastSkill();
        }
        else if (Input.GetKeyUp(keyCode))
        {
            BeginCoolDown();
        }
    }
    public void ActivateAbility(GameObject player,KeyCode keyCode)
    {
        playerClone = player.GetComponent<Player>();
        if(this.keyCode == KeyCode.None)
        {
            this.keyCode = keyCode;
        }
        if(this.keyCode == keyCode)
        {
            if (state == AbilityState.ready)
            {
                //playerClone.IsAbility = true;
                ability.Activate(playerClone.gameObject);
                state = AbilityState.cast;
            }
        }

    }
    public void CastSkill()
    {
        if (state == AbilityState.cast)
        {
            ability.CastSkill(playerClone.gameObject);
        }
    }

    public void BeginCoolDown()
    {
        if (playerClone.IsAbility) 
        {
            state = AbilityState.active;
            ability.BeginCooldown(playerClone.gameObject);
            StartCoroutine(Activated());
            StartCoroutine(CoolDown());
            keyCode = KeyCode.None;
        }
    }
    IEnumerator Activated()
    {
        yield return new WaitForSeconds(activeTime);
    }
    IEnumerator CoolDown()
    {
        state = AbilityState.cooldown;
        //playerClone.IsAbility = false;
        Debug.Log(playerClone.IsAbility);
        yield return new WaitForSecondsRealtime(cooldownTime);
        state = AbilityState.ready;
    }
}