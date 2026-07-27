using UnityEngine;

public class Core : CoreBase
{
    [SerializeField] public Player Player { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Player = GetComponentInParent<Player>();
        
    }
}
