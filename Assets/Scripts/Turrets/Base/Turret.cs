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

        //if there is a list, check if stop timer on previous face
        if(turretsOnFace.Count > 0)
            CheckPreviousFace();

        //if there is a limit of turrets on same face, check if there are other turrets on new face
        if (GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace > 0)
            CheckTurretsOnSameFace();
    }

    public override void BuildTurret(Cell cellOwner)
    {
        base.BuildTurret(cellOwner);

        //if destroy turret when no move and time greater than 0, init timer to destroy turret
        if (GameManager.instance.levelManager.levelConfig.DestroyTurretWhenNoMove && timeBeforeDestroy > 0)
            InitTimer();

        //if there is a limit of turrets on same face, check if there are other turrets on same face
        if (GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace > 0)
            CheckTurretsOnSameFace();
    }

    public override void RemoveTurret()
    {
        base.RemoveTurret();

        //be sure to remove timer to destroy turret if no move
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

        //foreach cell on this face
        if (GameManager.instance.levelManager.levelConfig.GeneratorActiveAllFace)
        {
            foreach (Cell cell in GameManager.instance.world.Cells.Values.Where(x => x.coordinates.face == CellOwner.coordinates.face))
            {
                //if there is a turret, is a generator and is active
                if (cell.turret != null && cell.turret is Generator && cell.turret.IsActive)
                    generatorsOnThisFace++;
            }
        }
        //else foreach cell around
        else
        {
            foreach (Cell cell in GameManager.instance.world.GetCellsAround(CellOwner.coordinates))
            {
                //if there is a turret, is a generator and is active
                if (cell.turret != null && cell.turret is Generator && cell.turret.IsActive)
                    generatorsOnThisFace++;
            }
        }

        return generatorsOnThisFace;
    }

    #endregion

    #region timer before destroy

    DestroyTurretWhenNoMove destroyTurretWhenNoMove = new DestroyTurretWhenNoMove();

    public System.Action<float> updateTimeBeforeDestroy;

    void InitTimer()
    {
        destroyTurretWhenNoMove.InitTimer(this, timeBeforeDestroy);
    }

    void RemoveTimer()
    {
        destroyTurretWhenNoMove.RemoveTimer();
    }

    #endregion

    #region no turrets on same face

    DestroyTurretsOnSameFace destroyTurretsOnSameFace = new DestroyTurretsOnSameFace();
    static Dictionary<EFace, List<Turret>> turretsOnFace = new Dictionary<EFace, List<Turret>>();
    EFace previousFace;

    public System.Action<List<Turret>> startTimerTurretsOnSameFace;
    public System.Action<float> updateFeedbackTurretsOnSameFace;
    public System.Action<EFace, List<Turret>> updateNumberOfTurretsOnSameFace;
    public System.Action<EFace> stopTimerTurretsOnSameFace;

    void CheckPreviousFace()
    {
        //if no key, then there is no timer to stop or update
        if (turretsOnFace.ContainsKey(previousFace) == false)
            return;

        //if not exceed limit (-1 because this turret is not anymore on this face)
        if (turretsOnFace[previousFace].Count - 1 <= GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace)
        {
            //foreach turret in the list, stop timer
            foreach(Turret t in turretsOnFace[previousFace])
            {
                t.destroyTurretsOnSameFace.StopTimer();
            }

            //remove turrets from the list
            turretsOnFace[previousFace].Clear();

            //EVENT stop timer
            stopTimerTurretsOnSameFace?.Invoke(previousFace);
        }
        //if still exceed the limit, don't stop timer for others turrets
        else
        {
            //but stop timer for this turret and remove from the list
            destroyTurretsOnSameFace.StopTimer();
            RemoveFromTurretsOnFace();

            //EVENT update positions
            updateNumberOfTurretsOnSameFace?.Invoke(previousFace, turretsOnFace[previousFace]);
        }
    }

    void CheckTurretsOnSameFace()
    {
        //save previous face
        previousFace = CellOwner.coordinates.face;

        
        Turret[] turrets;

        //find turrets on this face of same type (check if same prefab)
        if (GameManager.instance.levelManager.levelConfig.OnlyIfSameType)
        {
            turrets = FindObjectsOfType<Turret>().Where(x => x.CellOwner.coordinates.face == CellOwner.coordinates.face && x.CellOwner.TurretToCreate == CellOwner.TurretToCreate).ToArray();
        }
        //else find every turrets on this face, without check type
        else
        {
            turrets = FindObjectsOfType<Turret>().Where(x => x.CellOwner.coordinates.face == CellOwner.coordinates.face).ToArray();
        }

        //if exceed limit
        if (turrets.Length > GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace)
        {
            //if there are already timers running on this face, restart all
            if(turretsOnFace.ContainsKey(CellOwner.coordinates.face) && turretsOnFace[CellOwner.coordinates.face].Count > 0)
            {
                turretsOnFace[CellOwner.coordinates.face].Clear();
            }

            //foreach turret, add to static list and start timer(or restart if already running)
            foreach (Turret t in turrets)
            {
                AddToTurretsOnFace(t);
                t.destroyTurretsOnSameFace.StartTimer(t, GameManager.instance.levelManager.levelConfig.TimeBeforeDestroyTurretsOnSameFace);
            }

            //EVENT start timer (or restart)
            startTimerTurretsOnSameFace?.Invoke(turretsOnFace[CellOwner.coordinates.face]);
        }
    }

    void AddToTurretsOnFace(Turret turret)
    {
        //create key if necessary
        if (turretsOnFace.ContainsKey(CellOwner.coordinates.face) == false)
        {
            turretsOnFace.Add(CellOwner.coordinates.face, new List<Turret>());
        }

        //add to list
        turretsOnFace[CellOwner.coordinates.face].Add(turret);
    }

    void RemoveFromTurretsOnFace()
    {
        //if in the dictionary
        if (turretsOnFace.ContainsKey(previousFace))
        {
            //remove from the list
            turretsOnFace[previousFace].Remove(this);
        }
    }

    #endregion
}
