using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Enemy Graphics/More Turrets To Die Graphics")]
public class MoreTurretsToDieGraphics : EnemyGraphics
{
    [Header("More Turrets To Die")]
    [SerializeField] LineRenderer linePrefab = default;

    EnemyMoreTurretsToDie logic;
    LineRenderer lineFeedback;

    void Start()
    {
        //get logic reference
        logic = GetComponent<EnemyMoreTurretsToDie>();

        //instantiate line feedback
        lineFeedback = Instantiate(linePrefab, transform);
    }

    void Update()
    {
        //set line feedback
        SetPositions();
    }

    void SetPositions()
    {
        if (lineFeedback && logic.turretsAiming != null)
        {
            //get position of every turret
            List<Vector3> positions = new List<Vector3>();
            foreach (Turret t in logic.turretsAiming)
            {
                positions.Add(t.GetComponent<TurretGraphics>().LinePosition.position);
                positions.Add(transform.position);  //add also this position, so every line go from a turret to this enemy
            }

            //set positions
            lineFeedback.positionCount = positions.Count;
            lineFeedback.SetPositions(positions.ToArray());
        }
    }
}
