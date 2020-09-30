using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/World/Random World Config", fileName = "Random World Config")]
public class RandomWorldConfig : ScriptableObject
{
    [Tooltip("Loop or do only N times?")]
    public bool Loop = false;
    [Tooltip("How many times you want to randomize")]
    public int RandomizeTimes = 15;
    [Tooltip("Time to wait before start to randomize")]
    public float TimeBeforeRandomize = 1;
    [Tooltip("Time for the animation")]
    public float RotationTime = 0.1f;
    [Tooltip("Time between every rotation")]
    public float TimeBetweenRotation = 0f;
}
