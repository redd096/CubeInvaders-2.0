using UnityEngine;
using System.Linq;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy More Turrets To Die")]
[RequireComponent(typeof(MoreTurretsToDieGraphics))]
public class EnemyMoreTurretsToDie : Enemy
{
    [Header("More Turrets To Die")]
    [Tooltip("Number of turrets who must aim this enemy to do damage")] [SerializeField] int numberOfTurretsWhoMustAim = 2;

    public TurretShooter[] turretsAiming { get; private set; }

    protected void Update()
    {
        //find every turret shooter aiming at this enemy
        turretsAiming = FindObjectsOfType<TurretShooter>().Where(x => x.EnemyToAttack == this).ToArray();
    }

    public override void GetDamage(float damage, TurretShot whoHit)
    {
        //if reached number of enemies aiming to this enemy, get damage
        if (turretsAiming.Length >= numberOfTurretsWhoMustAim)
        {
            base.GetDamage(damage, whoHit);
        }
    }
}
