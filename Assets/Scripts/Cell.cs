using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Important")]
    public EBiomes biome;
    public Turret turretOnThisBiome;

    [Header("Debug")]
    public Coordinates coordinates;
}
