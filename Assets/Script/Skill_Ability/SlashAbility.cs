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

    public PlayerInputHandler playerInputHandler;
    #endregion
    public override void Enter(Player player)
    {
        base.Enter(player);
        player.Core.GetCoreComponent(out playerInputHandler);
        positon = (Vector2)playerInputHandler.transform.position + playerInputHandler.DirectionMouseVector.normalized * 3;
        rotation = Quaternion.Euler(0, 0, playerInputHandler.AngleRotationPlayer);
        shoot = playerInputHandler.DirectionMouseVector;
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
