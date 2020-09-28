using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    [Tooltip("Keep pressed for this time to end strategic phase")] public float timeToEndStrategic = 1.5f;
}
