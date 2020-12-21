using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy More Turrets To Die")]
public class EnemyMoreTurretsToDie : Enemy
{
    [Header("More Turrets To Die")]
    [SerializeField] LineRenderer linePrefab = default;
    [Tooltip("Number of turrets who must aim this enemy to do damage")] [SerializeField] int numberOfTurretsWhoMustAim = 2;

    LineRenderer lineFeedback;
    TurretShooter[] turretsAiming;

    protected override void Awake()
    {
        base.Awake();

        //instantiate line feedback but deactivate
        lineFeedback = Instantiate(linePrefab, transform);
    }

    protected void Update()
    {
        //find every turret shooter aiming at this enemy
        turretsAiming = FindObjectsOfType<TurretShooter>().Where(x => x.EnemyToAttack == this).ToArray();

        //update graphics
        SetPositions();
    }

    public override void GetDamage(float damage)
    {
        //if reached number of enemies aiming to this enemy, get damage
        if (turretsAiming.Length >= numberOfTurretsWhoMustAim)
        {
            base.GetDamage(damage);
        }
    }

    void SetPositions()
    {
        if (lineFeedback)
        {
            //get position of every turret
            List<Vector3> positions = new List<Vector3>();
            foreach (Turret t in turretsAiming)
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
