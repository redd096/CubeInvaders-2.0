using UnityEngine;

[System.Serializable]
public struct WaveStruct
{
    public Enemy[] EnemiesPrefabs;
}

[CreateAssetMenu(menuName = "Cube Invaders/Level/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Important")]
    [Tooltip("The player can recreate a destroyed cell")] public bool CanRecreateCell = false;
    [Tooltip("Randomize world at start")] public bool RandomizeWorldAtStart = true;

    [Header("Wave")]
    public float TimeBetweenSpawns = 3;
    public float distanceFromWorld = 30;
    public WaveStruct[] Waves;
}
