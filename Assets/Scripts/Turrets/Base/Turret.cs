using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Turret : BuildableObject
{
    [Header("Turret Modifier")]
    [Tooltip("Number of generators necessary to activate (0 = no generator)")] [Min(0)] [SerializeField] int needGenerator = 1;
    [Tooltip("Timer to destroy if player doesn't move it (0 = no destroy)")] [Min(0)] [SerializeField] float timeBeforeDestroy = 5;

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

    protected override void OnEndRotation()
    {
        base.OnEndRotation();

        //check if there are other turrets on same face
        CheckTurretsOnSameFace();
    }

    public override void BuildTurret(Cell cellOwner)
    {
        base.BuildTurret(cellOwner);

        //init timer to destroy turret
        InitTimer();

        //check if there are other turrets on same face
        CheckTurretsOnSameFace();
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

    #region no turrets on same face

    void CheckTurretsOnSameFace()
    {
        //if level config has limit of turrets on same face
        if(GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace > 0)
        {
            //find turrets on this face
            Turret[] turrets = FindObjectsOfType<Turret>().Where(x => x.CellOwner.coordinates.face == CellOwner.coordinates.face).ToArray();

            //if exceed limit, remove every turret
            if (turrets.Length > GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace)
            {
                foreach(Turret t in turrets)
                {
                    t.RemoveTurret();
                }
            }
        }
    }

    #endregion
}
