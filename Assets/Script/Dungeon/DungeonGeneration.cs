using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGenerationData.asset", menuName = "DungeonGenerationData/Dungeon Data")]

public class DungeonGeneration : ScriptableObject
{
    public int numberOfCrawlers, 
        iterationMin, iterationMax;

}
