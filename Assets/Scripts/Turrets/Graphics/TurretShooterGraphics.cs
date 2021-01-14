using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Turret Shooter Graphics")]
public class TurretShooterGraphics : TurretGraphics
{
    TurretShooter turretShooter;

    protected override void Awake()
    {
        base.Awake();

        //get logic component as turret shooter
        turretShooter = buildableObject as TurretShooter;
    }

    protected override Enemy GetEnemy()
    {
        //get enemy from logic component
        return turretShooter.EnemyToAttack;
    }

    #region events

    protected override void AddEvents()
    {
        base.AddEvents();

        turretShooter.onShoot += OnShoot;
    }

    protected override void RemoveEvents()
    {
        base.RemoveEvents();

        turretShooter.onShoot -= OnShoot;
    }

    void OnShoot()
    {
        //animation on shoot
    }

    #endregion

}
