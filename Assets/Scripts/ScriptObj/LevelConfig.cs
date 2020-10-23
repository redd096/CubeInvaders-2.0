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

    [Header("Modifier")]
    [Tooltip("How many rotations at time")] [Min(1)] public int numberRotations = 1;

    [Header("Wave")]
    public float TimeBetweenSpawns = 3;
    public float distanceFromWorld = 30;
    public WaveStruct[] Waves;
}
