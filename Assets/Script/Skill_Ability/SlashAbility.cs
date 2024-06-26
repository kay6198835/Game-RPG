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
    public override void Activate()
    {
        //positon = (Vector2)player.InputHandler.transform.position + player.InputHandler.DirectionLookVector.normalized * 3;
        //rotation = Quaternion.Euler(0, 0, player.InputHandler.AngleRotationPlayer);
        //shoot = player.InputHandler.DirectionLookVector;
        base.Activate();
    }
    public override void Do()
    {
        base.Do();
        Instantiate(slashPrefab, positon, rotation).gameObject.
            GetComponent<Projectile>().SetVelocity (shoot*speedSlash);
    }
    public override void Enter(NewPlayer player)
    {
        base.Enter(player);
        positon = (Vector2)player.InputHandler.transform.position + player.InputHandler.DirectionMouseVector.normalized * 3;
        rotation = Quaternion.Euler(0, 0, player.InputHandler.AngleRotationPlayer);
        shoot = player.InputHandler.DirectionMouseVector;
    }
}
