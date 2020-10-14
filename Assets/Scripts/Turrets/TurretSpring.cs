using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret/Turret Spring")]

public class TurretSpring : TurretShooter
{
    [SerializeField] int rotationCounter;
    [SerializeField] int max;

    protected override void Update()
    {
        if (rotationCounter >= max)
        {
            base.Update();
        }
    }

    protected override void Attack()
    {
        base.Attack();

        rotationCounter = 0;
    }

    protected override void OnEndRotation()
    {
        rotationCounter++;

        base.OnEndRotation();
    }
}

