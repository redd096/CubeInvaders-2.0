using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret/Turret Spring")]
[SelectionBase]
public class TurretSpring : TurretShooter
{
    int rotationCounter = 0;
    [SerializeField] int numberRotation = 0;

    protected override void Update()
    {
        //if reached number of rotation, can try attack
        if (rotationCounter >= numberRotation)
        {
            base.Update();
        }
    }

    protected override void Attack()
    {
        base.Attack();

        //reset counter
        rotationCounter = 0;
    }

    protected override void OnEndRotation()
    {
        base.OnEndRotation();

        //increase counter
        rotationCounter++;
    }
}

