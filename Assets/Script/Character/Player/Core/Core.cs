using UnityEngine;

public class Core : CoreBase
{
    [SerializeField] public Player Player { get; private set; }
    public override CharacterData Data => Player != null ? Player.Data : GetComponentInParent<Player>().Data;

    protected override void Awake()
    {
        base.Awake();
        Player = GetComponentInParent<Player>();
        
    }
}
