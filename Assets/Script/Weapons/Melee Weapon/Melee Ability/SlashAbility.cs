using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Slash Ability")]
public class SlashAbility : AbilitySO
{
    [Header("Stats")]
    [SerializeField] private GameObject slashPrefab;
    private Vector3 shoot;
    [SerializeField] private float speedSlash;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private Vector2 positon;
    #region
    public GameObject SlashPrefab { get => slashPrefab;}
    public Vector3 Shoot { get => shoot; }
    public float SpeedSlash { get => speedSlash; }
    #endregion
    public override void Activate(NewPlayer player)
    {
        positon = (Vector2)player.InputHandler.transform.position + player.InputHandler.DirectionVector.normalized * 3;
        rotation = Quaternion.Euler(0, 0, player.InputHandler.AngleSin);
        shoot = player.InputHandler.DirectionVector;
        base.Activate(player);
    }
    public override void DoAbility()
    {
        base.DoAbility();
        Instantiate(slashPrefab, positon, rotation).gameObject.
            GetComponent<Projectile>().SetXVelocity (shoot);
    }
}
