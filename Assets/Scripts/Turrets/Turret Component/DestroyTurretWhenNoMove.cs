using System.Collections;
using UnityEngine;

public class DestroyTurretWhenNoMove
{
    Turret turret;
    float timeBeforeDestroy;

    Coroutine timerBeforeDestroy;

    //on build turret, init this script (init timer)

    //on start assault, start timer
    //on rotate, reset timer (only if timer is started, so is in assault phase)
    //on end assault, stop timer

    //on remove turret, remove this script (remove timer)

    public void InitTimer(Turret turret, float timeBeforeDestroy)
    {
        //get references
        this.turret = turret;
        this.timeBeforeDestroy = timeBeforeDestroy;

        //add events
        AddEvents();
    }

    public void RemoveTimer()
    {
        //stop timer
        if (turret && timerBeforeDestroy != null)
        {
            turret.StopCoroutine(timerBeforeDestroy);
        }

        //remove events
        RemoveEvents();
    }

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase += OnEndAssaultPhase;

        if(turret)
            turret.CellOwner.onWorldRotate += OnWorldRotate;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase -= OnEndAssaultPhase;

        if (turret)
            turret.CellOwner.onWorldRotate -= OnWorldRotate;
    }

    void OnStartAssaultPhase()
    {
        //start timer
        StartTimer();
    }

    void OnWorldRotate(Coordinates coordinates)
    {
        //if timer is started (so in assault phase)
        if (timerBeforeDestroy != null)
        {
            //stop
            StopTimer();

            //and restart it
            StartTimer();
        }
    }

    void OnEndAssaultPhase()
    {
        //stop timer
        StopTimer();
    }

    #endregion

    #region private API

    void StartTimer()
    {
        //start timer
        if (turret)
        {
            timerBeforeDestroy = turret.StartCoroutine(TimerBeforeDestroy());
        }
    }

    void StopTimer()
    {
        //stop timer
        if (turret && timerBeforeDestroy != null)
        {
            turret.StopCoroutine(timerBeforeDestroy);
            timerBeforeDestroy = null;
        }
    }

    IEnumerator TimerBeforeDestroy()
    {
        //wait
        yield return new WaitForSeconds(timeBeforeDestroy);

        //than remove turret
        if (turret)
        {
            turret.RemoveTurret();
        }
    }

    #endregion
}
