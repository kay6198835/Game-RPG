using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newPLayerData",menuName ="Data/PLayer Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private LayerMask layerMask;
    [Header("Move State")]
    public float movementVelocities = 10f;
    private void Awake()
    {
        layerMask = LayerMask.GetMask("Enemy");
    }
}
