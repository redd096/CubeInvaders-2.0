﻿using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/Level/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Important")]
    [Tooltip("The player can recreate a destroyed cell")] public bool CanRecreateCell = false;
    [Tooltip("Randomize world at start")] public bool RandomizeWorldAtStart = true;

    [Header("Modifier")]
    [Tooltip("How many rotations at time")] [Min(1)] public int NumberRotations = 1;
    [Tooltip("Turret need generator to activate")] public bool TurretsNeedGenerator = false;
    [Tooltip("Size of the selector, to select one cell or more")] [Min(1)] public int SelectorSize = 1;
    [Tooltip("Destroy turret after few seconds that player doesn't move it")] public bool DestroyTurretWhenNoMove = false;
    [Tooltip("If you put more turret of same type on same face, they explode")] public bool NoTurretsOnSameFace = false;
}
