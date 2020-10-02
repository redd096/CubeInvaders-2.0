using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooterGraphics : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] Transform objectToRotate = default;

    TurretShooter turretShooter;

    void Start()
    {
        //get turret shooter
        turretShooter = GetComponent<TurretShooter>();

        //set event for OnShoot
        turretShooter.onShoot += OnShoot;
    }

    void OnDestroy()
    {
        //remove event for OnShoot
        turretShooter.onShoot -= OnShoot;
    }

    void Update()
    {
        //if is active, animate it
        if(turretShooter.IsActive)
            Animation();
    }

    void Animation()
    {
        //need model for animation - and an enemy to attack
        if (objectToRotate == null) return;

        //find up direction (from model to enemy)
        Vector3 upDirection;
        if (turretShooter.EnemyToAttack)
            upDirection = (turretShooter.EnemyToAttack.transform.position - objectToRotate.position).normalized;
        else
            upDirection = turretShooter.CellOwner.transform.up;

        //get new rotation
        Quaternion upRotation = Quaternion.FromToRotation(objectToRotate.up, upDirection) * objectToRotate.rotation;
        objectToRotate.rotation = upRotation;
    }

    void OnShoot()
    {
        //animation on shoot
    }
}
