using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret Graphics/Turret Shooter Graphics")]
public class TurretShooterGraphics : BuildableGraphics
{
    TurretShooter turretShooter;

    protected override void Start()
    {
        base.Start();

        //get logic component as turret shooter
        turretShooter = buildableObject as TurretShooter;

        AddEvents();
    }

    void OnDestroy()
    {
        RemoveEvents();
    }

    protected override Enemy GetEnemy()
    {
        //get enemy from logic component
        return turretShooter.EnemyToAttack;
    }

    #region events

    void AddEvents()
    {
        turretShooter.onShoot += OnShoot;
    }

    void RemoveEvents()
    {
        turretShooter.onShoot -= OnShoot;
    }

    void OnShoot()
    {
        //animation on shoot
    }

    #endregion

}
