using UnityEngine;

[System.Serializable]
public struct WaveStruct
{
    public LevelConfig LevelConfig;
    public BiomesConfig BiomesConfig;
    public Enemy[] EnemiesPrefabs;
}

[CreateAssetMenu(menuName = "Cube Invaders/Level/Wave Config", fileName = "Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Wave")]
    public float TimeBetweenSpawns = 3;
    public float distanceFromWorld = 30;
    public WaveStruct[] Waves;
}