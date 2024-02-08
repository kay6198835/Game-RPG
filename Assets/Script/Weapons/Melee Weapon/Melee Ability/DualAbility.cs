using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Dual Ability")]
public class DualAbility : AbilitySO
{
    [Header("Stats Dual Ability")]
    [SerializeField] private Vector2 sizeAttack;
    [SerializeField] private Vector2 worldpositon;
    [SerializeField] private float rangeAbility;
    [SerializeField] private float delayTime;
    [SerializeField] private int damage;
    [SerializeField] private float manaLost;
    [SerializeField] int randomDirection;
    //[SerializeField] private Player playerClone;
    [SerializeField] private GameObject skillModel;
    public override void Activate(GameObject player)
    {
        base.Activate(player);
        //playerClone.GetComponent<Player>().RigidbodyCharacter.bodyType = RigidbodyType2D.Static;
        sizeAttack = new Vector2(rangeAbility, 0.2f);
        worldpositon = player.transform.TransformPoint(Vector3.zero);
        //playerClone.IsFrezee = true;
    }
    public override void CastSkill(GameObject player)
    {
        if (playerClone.ManaCurrent <= 0)
        {
            return;
        }
        base.CastSkill(player);
        if (currentTime >= timeStarCast + delayTime)
        {
            RandomAttack(player);
            //randomDirection = (int)Random.Range(0, 8);
            //playerClone.Animator.runtimeAnimatorController = animators[randomDirection];
            playerClone.Animator.runtimeAnimatorController = animators[(int)playerClone.Animator.GetFloat("Direction")];
            Debug.Log(playerClone.Animator.runtimeAnimatorController + " " + randomDirection);
            playerClone.GetComponent<Player>().AnimationManager.Animation_5_Ability();
            //SetBool 
            //playerClone.Animator.Play("Ability", 0, 0);
            if (playerClone.IsAbility)
            {
                playerClone.ManaCurrent -= (manaLost * 1 / 60);
                playerClone.ManaBar.SetValue(playerClone.ManaCurrent);
            }
        }
    }
    private void RandomAttack(GameObject player)
    {
        //Random Positon
        Vector2 randomVector = Random.onUnitSphere;
        Vector2 randomPosition = randomVector.normalized * rangeAbility;
        //Rotation line attack
        Vector2 dir = randomPosition - randomPosition / 2;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        playerClone.GetComponent<PlayerMovement>().AngleCalculate(randomPosition);
        Attack(randomPosition, angle ,rotation, player);
    }
    void Attack(Vector2 randomPosition, float angle, Quaternion rotation, GameObject player)
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(worldpositon + randomPosition / 2, sizeAttack, angle, layerMask);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage, player);
            }
        }
        GameObject skill = Instantiate(skillModel, worldpositon + randomPosition / 2, rotation);
        skill.transform.localScale = (Vector3)sizeAttack;
    }
    public override void BeginCooldown(GameObject player)
    {
        base.BeginCooldown(player);
        //playerClone.IsFrezee = false;
        //playerClone.GetComponent<Player>().RigidbodyCharacter.bodyType = RigidbodyType2D.Dynamic;
        //playerClone.GetComponent<Player>().AnimationManager.Animation_5_OfAbility();
    }
}
