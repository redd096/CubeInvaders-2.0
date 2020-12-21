using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/Level/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Important")]
    [Tooltip("The player can recreate a destroyed cell")] public bool CanRecreateCell = false;
    [Tooltip("Randomize world at start")] public bool RandomizeWorldAtStart = true;

    [Header("Modifier")]
    [Tooltip("How many rotations at time")] [Min(1)] public int NumberRotations = 1;
    [Tooltip("Size of the selector, to select one cell or more")] [Min(1)] public int SelectorSize = 1;
    [Tooltip("Destroy turret after few seconds that player doesn't move it")] public bool DestroyTurretWhenNoMove = false;

    [Header("Modifier - Generator")]
    [Tooltip("Turret need generator to activate")] public bool TurretsNeedGenerator = false;
    [Tooltip("Activate every turret on this face, or only turrets around")] public bool GeneratorActiveAllFace = true;

    [Header("Modifier - Limit of turrets on same face")]
    [Tooltip("Limit of turrets on same face, if exceed explode turrets (0 = no limits)")] [Min(0)] public int LimitOfTurretsOnSameFace = 0;
    [Tooltip("Timer to destroy if there are more turrets on same face")] [Min(0)] public float TimeBeforeDestroyTurretsOnSameFace = 2;
    [Tooltip("Limit of turrets on same face, only if is the same type of turret")] public bool OnlyIfSameType = true;
    [Tooltip("When draw line feedback, close line or keep open?")] public bool CloseLineFeedback = true;
}
