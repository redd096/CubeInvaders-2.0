using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : BuildableObject
{
    [Header("Turret Modifier")]
    [SerializeField] bool needGenerator = true;

    public override void TryActivateTurret()
    {
        //if doesn't need generator or there is a generator around, activate it
        if (GameManager.instance.levelManager.levelConfig.turretsNeedGenerator == false || needGenerator == false || CheckGeneratorAround())
            base.TryActivateTurret();
    }

    public override void TryDeactivateTurret()
    {
        //if need generator and there is no generator around, deactive it
        if(GameManager.instance.levelManager.levelConfig.turretsNeedGenerator && needGenerator && CheckGeneratorAround() == false)
            base.TryDeactivateTurret();
    }

    bool CheckGeneratorAround()
    {
        Vector2Int[] directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

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
                        return true;
                }
            }
        }

        return false;
    }
}
