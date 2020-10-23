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
        return GameManager.instance.levelManager.levelConfig.turretsNeedGenerator && needGenerator > 0;
    }

    int CheckGeneratorsAround()
    {
        Vector2Int[] directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
        int generatorsOnThisFace = 0;

        //foreach direction
        foreach(Vector2Int direction in directions)
        {
            //if there is a cell and is != null
            if (GameManager.instance.world.Cells.ContainsKey(CellOwner.coordinates + direction))
            {
                Cell cell = GameManager.instance.world.Cells[CellOwner.coordinates + direction];
                if (cell != null)
                {
                    //if there is a turret, is a generator and is active
                    if (cell.turret != null && cell.turret is Generator && cell.turret.IsActive)
                        generatorsOnThisFace++;
                }
            }
        }

        return generatorsOnThisFace;
    }

    #endregion
}
