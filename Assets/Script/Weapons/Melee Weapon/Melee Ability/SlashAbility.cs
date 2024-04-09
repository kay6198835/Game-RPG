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
    private Quaternion rotation;
    private Vector2 positon;
    #region
    public GameObject SlashPrefab { get => slashPrefab;}
    public Vector3 Shoot { get => shoot; }
    public float SpeedSlash { get => speedSlash; }
    #endregion
    public override void Activate()
    {
        positon = (Vector2)player.InputHandler.transform.position + player.InputHandler.DirectionVector.normalized * 3;
        rotation = Quaternion.Euler(0, 0, player.InputHandler.AngleSin);
        shoot = player.InputHandler.DirectionVector;
        base.Activate();
    }
    public override void Do()
    {
        base.Do();
        Instantiate(slashPrefab, positon, rotation).gameObject.
            GetComponent<Projectile>().SetXVelocity (shoot);
    }
}
