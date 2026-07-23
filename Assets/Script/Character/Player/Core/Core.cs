using UnityEngine;

public class Core : CoreBase
{
    [SerializeField] public Player Player { get; private set; }

    private void Awake()
    {
        Player = GetComponentInParent<Player>();
        
    }
}
