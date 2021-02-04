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
    [Tooltip("To set speed in animation. From 0 to 1 time and value, where value is rotation posizione in time")] 
    public AnimationCurve RotationAnimationCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });
    [Tooltip("Time between every rotation")]
    public float TimeBetweenRotation = 0f;
}
