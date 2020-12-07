using System.Collections;
using UnityEngine;

public class DestroyTurretWhenNoMove
{
    Turret turret;
    float timeBeforeDestroy;

    Coroutine timerBeforeDestroy_coroutine;

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
        if (turret && timerBeforeDestroy_coroutine != null)
        {
            turret.StopCoroutine(timerBeforeDestroy_coroutine);
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
        if (timerBeforeDestroy_coroutine != null)
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
            timerBeforeDestroy_coroutine = turret.StartCoroutine(TimerBeforeDestroy_Coroutine());
        }
    }

    void StopTimer()
    {
        //stop timer
        if (turret && timerBeforeDestroy_coroutine != null)
        {
            turret.StopCoroutine(timerBeforeDestroy_coroutine);
            timerBeforeDestroy_coroutine = null;
        }
    }

    IEnumerator TimerBeforeDestroy_Coroutine()
    {
        float timer = Time.time + timeBeforeDestroy;

        //wait and update timer
        while (Time.time < timer)
        {
            if (turret)
            {
                float timeRemaining = timer - Time.time;
                turret.updateTimeBeforeDestroy?.Invoke(1 - (timeRemaining / timeBeforeDestroy));  //from 0 to 1
            }

            yield return null;
        }

        //than remove turret
        if (turret)
        {
            turret.RemoveTurret();
        }
    }

    #endregion
}
