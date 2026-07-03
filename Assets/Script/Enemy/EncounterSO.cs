using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEncounter", menuName = "Data/Encounter")]
public class EncounterSO : ScriptableObject
{
    [SerializeField] private RoomType roomType;
    [SerializeField] private List<EncounterEntry> entries = new List<EncounterEntry>();

    public RoomType RoomType { get => roomType; }
    public List<EncounterEntry> Entries { get => entries; }
}

[System.Serializable]
public class EncounterEntry
{
    public Entity prefab;
    [Range(1, 10)] public int count = 1;
    // reserved for weighted random selection when multi-wave encounters land
    [Range(1, 100)] public int weight = 1;
}
