using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Slash Ability")]
public class SlashAbility : AbilitySO
{
    [Header("Stats")]
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private Projectile slashGO;
    [SerializeField] private Vector3 shoot;
    [SerializeField] private float speedSlash;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private Vector2 positon;
    #region
    public GameObject SlashPrefab { get => slashPrefab;}
    public Projectile SlashGO { get => slashGO;}
    public Vector3 Shoot { get => shoot; }
    public float SpeedSlash { get => speedSlash; }
    public Quaternion Rotation { get => rotation; }
    public Vector2 Positon { get => positon; }
    #endregion
    public override void Activate(NewPlayer player)
    {
        positon = (Vector2)player.InputHandler.transform.position + player.InputHandler.DirectionVector.normalized * 3;
        rotation = Quaternion.Euler(0, 0, player.InputHandler.AngleSin);
        shoot = player.InputHandler.DirectionVector;
        base.Activate(player);
        //shoot = new Vector3(0, 0, 0);
        //rotation = new Quaternion(xRotation, yRotation, zRotation, wRotation);

    }
    public override void DoAbility()
    {

        Debug.Log("Do Ability");
        base.DoAbility();
        Instantiate(slashPrefab, positon, rotation).gameObject.
            GetComponent<Projectile>().SetVelocity(speedSlash * periodCastTime * shoot);
    }
    public void Attack(int attackIndex)
    {
        //Collider2D[] enemies = Physics2D.OverlapBoxAll(PositionAttack(player),
        //    new Vector2(distance, 0.5f), player.transform.localRotation.eulerAngles.z, layerMask);
        //foreach (var enemy in enemies)
        //{
        //    if (enemy.GetComponent<Enemy>() != null)
        //    {
        //        enemy.GetComponent<Enemy>().TakeDamage(damage, player);
        //    }
        //    if (currentLevel == levelMax)
        //    {
        //        if (enemy.GetComponent<Enemy>() != null)
        //        {
        //            enemy.GetComponent<Enemy>().Stun(stunTime);
        //        }
        //    }
        //}
    }
    private Vector2 PositionAttack(GameObject player)
    {
        //Vector2 mousePosition = player.GetComponent<PlayerMovement>().MousePosition;
        //Vector2 direction = mousePosition - (Vector2)player.transform.position;
        //Vector2 targetPosition = (Vector2)player.transform.position + direction.normalized * distance / 2;
        return Vector2.one;
    }
}
