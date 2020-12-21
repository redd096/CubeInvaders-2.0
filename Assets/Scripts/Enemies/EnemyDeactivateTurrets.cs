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

    Coroutine deactivateCoroutine;

    void Start()
    {
        StartCoroutine();
    }

    void StartCoroutine()
    {
        if (deactivateCoroutine != null)
            StopCoroutine(deactivateCoroutine);

        //start coroutine
        deactivateCoroutine = StartCoroutine(DeactivateCoroutine());
    }

    IEnumerator DeactivateCoroutine()
    {
        //wait then deactivate
        yield return new WaitForSeconds(howOftenDeactivate);

        //deactivate
        Deactivate();
    }

    void Deactivate()
    {
        //foreach one, deactivate
        foreach (BuildableObject b in FindObjectsToDeactivate())
        {
            b.Deactivate(durationEffect);
        }

        //restart coroutine
        StartCoroutine();
    }

    BuildableObject[] FindObjectsToDeactivate()
    {
        //find every buildable object on this face
        IEnumerable<BuildableObject> objectsToDeactivate = FindObjectsOfType<BuildableObject>().Where(x => x.CellOwner.coordinates.face == coordinatesToAttack.face);

        //remove generators
        if (deactivateGeneratorsToo == false)
        {
            objectsToDeactivate = objectsToDeactivate.Where(x => x is Generator == false);
        }

        //remove radars
        if (deactivateRadarsToo == false)
        {
            objectsToDeactivate = objectsToDeactivate.Where(x => x is Radar == false);
        }

        return objectsToDeactivate.ToArray();
    }
}
