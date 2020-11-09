using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : BuildableObject
{
    [Header("Turret Modifier")]
    [SerializeField] int needGenerator = 1;

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
}
