using UnityEngine;

[AddComponentMenu("Cube Invaders/Turret/Generator")]
public class Generator : BuildableObject
{
    #region override

    protected override void ActivateTurret()
    {
        base.ActivateTurret();

        //active turrets around
        CheckTurretsAround(true);
    }

    protected override void DeactivateTurret()
    {
        base.DeactivateTurret();

        //try deactivate turrets around
        CheckTurretsAround(false);
    }

    protected override void OnWorldRotate(Coordinates coordinates)
    {
        base.OnWorldRotate(coordinates);

        //try deactivate turrets around
        CheckTurretsAround(false);
    }

    protected override void OnEndRotation()
    {
        base.OnEndRotation();

        //active turrets around
        CheckTurretsAround(true);
    }

    #endregion

    #region private API

    void CheckTurretsAround(bool activate)
    {
        Vector2Int[] directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

        //foreach direction
        foreach (Vector2Int direction in directions)
        {
            //if there is a cell and is != null
            if (GameManager.instance.world.Cells.ContainsKey(CellOwner.coordinates + direction))
            {
                Cell cell = GameManager.instance.world.Cells[CellOwner.coordinates + direction];
                if (cell != null)
                {
                    ActivateDeactivate(cell, activate);
                }
            }
        }
    }

    void ActivateDeactivate(Cell cell, bool activate)
    {
        //if there is a turret, is a Turret, and is not a preview
        if (cell.turret != null && cell.turret is Turret && cell.turret.IsPreview == false)
        {
            //try activate or deactivate
            if (activate)
                cell.turret.TryActivateTurret();
            else
                cell.turret.TryDeactivateTurret();
        }
    }

    #endregion
}
