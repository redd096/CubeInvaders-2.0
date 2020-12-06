using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Turret : BuildableObject
{
    [Header("Turret Modifier")]
    [Tooltip("Number of generators necessary to activate (0 = no generator)")] [Min(0)] [SerializeField] int needGenerator = 1;
    [Tooltip("Timer to destroy if player doesn't move it (0 = no destroy)")] [Min(0)] [SerializeField] float timeBeforeDestroy = 5;
    [Tooltip("Limit of turrets of this type on same face, if exceed explode turrets (0 = no limits)")] [Min(0)] [SerializeField] int limitOfTurretsOnSameFace = 1;

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
        //if level config has turrets on same face and limits greater than 0
        if(GameManager.instance.levelManager.levelConfig.NoTurretsOnSameFace && limitOfTurretsOnSameFace > 0)
        {
            BuildableObject prefab = CellOwner.TurretToCreate;

            //find turrets on this face, with same type (check turret to create of cell owner)
            Turret[] turrets = FindObjectsOfType<Turret>().Where(x => x.CellOwner.coordinates.face == CellOwner.coordinates.face && x.CellOwner != null && x.CellOwner.TurretToCreate == prefab).ToArray();

            //if exceed limit, remove every turret
            if(turrets.Length > limitOfTurretsOnSameFace)
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
