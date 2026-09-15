using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "newPLayerData", menuName = "Data/PLayer Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private BaseStatsSO stats;
    public BaseStatsSO Stats { get => stats; }
    [field: SerializeField] public List<AbilityBinding> AbilityBindings { get; private set; } = new();
}
