using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : BuildableObject
{
    [Header("Turret Modifier")]
    [SerializeField] int needGenerator = 1;
    [Tooltip("Timer to destroy if player doesn't move it (0 = no destroy)")] [SerializeField] float timeBeforeDestroy = 5;

    public override void TryActivateTurret()
    {
        //if doesn't need generator or there are enough generators around, activate it
        if (NeedGenerator() == false || CheckGeneratorsAround() >= needGenerator)
            base.TryActivateTurret();
    }

    public override void TryDeactivateTurret()
    {
        //if need generator and there is no enough generators around, deactive it
        if(NeedGenerator() && CheckGeneratorsAround() < needGenerator)
            base.TryDeactivateTurret();
    }

    public override void BuildTurret(Cell cellOwner)
    {
        base.BuildTurret(cellOwner);

        //init timer to destroy turret
        InitTimer();
    }

    public override void RemoveTurret()
    {
        base.RemoveTurret();

        //remove timer to destroy turret
        RemoveTimer();
    }

    #region generator

    bool NeedGenerator()
    {
        return GameManager.instance.levelManager.levelConfig.TurretsNeedGenerator && needGenerator > 0;
    }

    int CheckGeneratorsAround()
    {
        int generatorsOnThisFace = 0;

        //foreach cell around
        foreach (Cell cell in GameManager.instance.world.GetCellsAround(CellOwner.coordinates))
        {
            //if there is a turret, is a generator and is active
            if (cell.turret != null && cell.turret is Generator && cell.turret.IsActive)
                generatorsOnThisFace++;
        }

        return generatorsOnThisFace;
    }

    #endregion

    #region timer before destroy

    DestroyTurretWhenNoMove destroyTurretWhenNoMove = new DestroyTurretWhenNoMove();

    void InitTimer()
    {
        //if level config has timer true and if timer greater than 0, start timer
        if (GameManager.instance.levelManager.levelConfig.DestroyTurretWhenNoMove && timeBeforeDestroy > 0)
        {
            destroyTurretWhenNoMove.InitTimer(this, timeBeforeDestroy);
        }
    }

    void RemoveTimer()
    {
        //if there is a timer, stop it
        if(destroyTurretWhenNoMove != null)
        {
            destroyTurretWhenNoMove.RemoveTimer();
        }
    }

    #endregion
}
