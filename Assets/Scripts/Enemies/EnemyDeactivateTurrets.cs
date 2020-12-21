using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Deactivate Turrets")]
public class EnemyDeactivateTurrets : Enemy
{
    [Header("Deactivate Turrets")]
    [Tooltip("How often this enemy deactivate turrets")] [SerializeField] float howOftenDeactivate = 5;
    [Tooltip("Timer before reactivate turrets")] [SerializeField] float durationEffect = 1;
    [SerializeField] bool deactivateGeneratorsToo = false;
    [SerializeField] bool deactivateRadarsToo = false;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        //if reached time to deactivate
        if(timer >= howOftenDeactivate)
        {
            timer = 0;
            Deactivate();
        }
    }

    void Deactivate()
    {
        //find every buildable object on this face
        BuildableObject[] objectsToDeactivate = FindObjectsOfType<BuildableObject>().Where(x => x.CellOwner.coordinates.face == coordinatesToAttack.face).ToArray();

        //remove generators
        if(deactivateGeneratorsToo == false)
        {
            objectsToDeactivate = objectsToDeactivate.Where(x => x is Generator == false).ToArray();
        }

        //remove radars
        if(deactivateRadarsToo == false)
        {
            objectsToDeactivate = objectsToDeactivate.Where(x => x is Radar == false).ToArray();
        }

        //foreach one, deactivate
        foreach (BuildableObject b in objectsToDeactivate)
            b.Deactivate(durationEffect);
    }
}
