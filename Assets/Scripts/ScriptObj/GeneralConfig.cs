using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/General Config", fileName = "General Config")]
public class GeneralConfig : ScriptableObject
{
    [Tooltip("Keep pressed for this time to end strategic phase")] public float TimeToEndStrategic = 1.5f;
    [Tooltip("Cell selector")] public GameObject Selector;
}
