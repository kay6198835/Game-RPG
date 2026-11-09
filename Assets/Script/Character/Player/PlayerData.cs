using UnityEngine;

// Stats, ability bindings and the default weapon live on CharacterData, shared with EntityData.
[CreateAssetMenu(fileName = "newPLayerData", menuName = "Data/PLayer Data/Base Data")]
public class PlayerData : CharacterData
{
    // NEED CHECK: 2026-11-09 (stat split) — ResetData() (wrote `stats.Level = 1` into the shared asset) removed:
    // it had no caller, and the profile asset is read-only data now.
}
