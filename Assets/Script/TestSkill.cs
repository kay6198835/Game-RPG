//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestSkill : MonoBehaviour
//{
//    [SerializeField] Player playerClone;
//    [Header("Stats Base")]
//    [SerializeField] protected new string name;
//    [SerializeField] protected float cooldownTime;
//    [SerializeField] protected float activeTime;
//    [SerializeField] protected float timeStarCast;
//    [SerializeField] protected float maxCastTime;
//    [SerializeField] protected float currentTime;
//    [SerializeField] protected float periodCastTime;
//    [Header("Stats Dual Ability")]
//    [SerializeField] private Vector2 sizeAttack;
//    [SerializeField] private Vector2 worldpositon;
//    [SerializeField] private float rangeAbility;
//    [SerializeField] private float delayTime;
//    [SerializeField] private int damage;
//    [SerializeField] private float manaLost;
//    [SerializeField] private GameObject skillModel;
//    [SerializeField] protected List<AnimatorOverrideController> animators;
//    // Update is called once per frame
//    private void Start()
//    {
//        timeStarCast = Time.time;
//        currentTime = Time.time;
//        playerClone.IsBlocking = true;
//        Debug.Log("Block");
//        playerClone.Weapon.ShieldArea.isTrigger = false;
//    }
//    void Update()
//    {
//        CastSkill(playerClone.gameObject);
//    }

//    public void CastSkill(GameObject player)
//    {
//        currentTime = Time.time;
//        playerClone.Animator.runtimeAnimatorController = animators[(int)playerClone.Animator.GetFloat("Direction")];
//        playerClone.GetComponent<Player>().AnimationManager.Animation_5_Ability();

//        //SetBool
//        //playerClone.Animator.Play("Ability", 0, 0);
//    }

//    private void RandomAttack(GameObject player)
//    {
//        //Random Positon
//        Vector2 randomVector = Random.onUnitSphere;
//        Vector2 randomPosition = randomVector.normalized * rangeAbility;
//        //Rotation line attack
//        Vector2 dir = randomPosition - randomPosition / 2;
//        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
//        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
//        playerClone.GetComponent<PlayerMovement>().AngleCalculate(randomPosition);
//        Attack(randomPosition, angle, rotation, player);
//    }
//    void Attack(Vector2 randomPosition, float angle, Quaternion rotation, GameObject player)
//    {
//        Collider2D[] enemies = Physics2D.OverlapBoxAll(worldpositon + randomPosition / 2, sizeAttack, angle);
//        foreach (Collider2D enemy in enemies)
//        {
//            if (enemy.GetComponent<Enemy>() != null)
//            {
//                enemy.GetComponent<Enemy>().TakeDamage(damage, player);
//            }
//        }
//        GameObject skill = Instantiate(skillModel, worldpositon + randomPosition / 2, rotation,transform);
//        skill.transform.localScale = (Vector3)sizeAttack;
//        Debug.Log("Skill");
//    }
//}
