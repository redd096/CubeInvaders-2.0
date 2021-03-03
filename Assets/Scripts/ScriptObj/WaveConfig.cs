using UnityEngine;

[System.Serializable]
public struct WaveStruct
{
    public LevelConfig LevelConfig;
    public float resourcesMax;
    public int IgnorePreviousFacesAtSpawn;
    public float TimeBetweenSpawns;
    public float DistanceFromWorld;
    public Enemy[] Enemies;
}

[CreateAssetMenu(menuName = "Cube Invaders/Level/Wave Config", fileName = "Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Wave")]
    public WaveStruct[] Waves;
}