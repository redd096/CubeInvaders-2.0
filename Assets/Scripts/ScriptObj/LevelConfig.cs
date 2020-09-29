using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    [Tooltip("Keep pressed for this time to end strategic phase")] public float TimeToEndStrategic = 1.5f;
    [Tooltip("The player can recreate a destroyed cell")] public bool CanRecreateCell = false;
}
