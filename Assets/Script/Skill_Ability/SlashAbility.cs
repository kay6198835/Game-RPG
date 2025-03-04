using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability SO/Activate Skill/Slash Ability")]
public class SlashAbility : ActivateSkill
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
    public override void Enter(NewPlayer player)
    {
        base.Enter(player);
        positon = (Vector2)player.InputHandler.transform.position + player.StatsBehavior.DirectionMouseVector.normalized * 3;
        rotation = Quaternion.Euler(0, 0, player.StatsBehavior.AngleRotationPlayer);
        shoot = player.StatsBehavior.DirectionMouseVector;
    }
    public override void Activate()
    {
        base.Activate();
    }
    public override void Do()
    {
        base.Do();
        Instantiate(slashPrefab, positon, rotation).gameObject.
            GetComponent<Projectile>().SetVelocity (shoot*speedSlash);
    }

}
