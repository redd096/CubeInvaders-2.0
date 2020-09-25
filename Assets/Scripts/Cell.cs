using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] EBiomes biome;
    [SerializeField] Turret turretOnThisBiome;

    [Header("Debug")]
    public Coordinates coordinates;

    public System.Action onWorldRotate;
}
