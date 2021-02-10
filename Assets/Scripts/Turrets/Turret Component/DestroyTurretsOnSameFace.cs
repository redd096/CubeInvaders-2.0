using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTurretsOnSameFace
{
    Turret turret;
    float timeBeforeDestroy;

    Coroutine timerBeforeDestroy_coroutine;

    //when there are more turrets on same face (end rotation or build new turret), start timer (or restart if already running)
    //when there are not too much turrets (rotate turrets on other face), stop timer

    //during timer, update feedback
    //on end timer, destroy turret

    public void StartTimer(Turret turret, float timeBeforeDestroy)
    {
        //get references
        this.turret = turret;
        this.timeBeforeDestroy = timeBeforeDestroy;

        //be sure is not already running (when restart timer)
        StopTimer();

        //start timer
        if (turret && turret.gameObject.activeInHierarchy)
        {
            timerBeforeDestroy_coroutine = turret.StartCoroutine(TimerBeforeDestroy_Coroutine());
        }
    }

    public void StopTimer()
    {
        //stop timer
        if (turret && timerBeforeDestroy_coroutine != null)
        {
            turret.StopCoroutine(timerBeforeDestroy_coroutine);
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
                turret.updateFeedbackTurretsOnSameFace?.Invoke(1 - (timeRemaining / timeBeforeDestroy));  //EVENT from 0 to 1
            }

            yield return null;
        }

        //than remove turret
        if (turret)
        {
            turret.RemoveTurret();
            turret.stopTimerTurretsOnSameFace?.Invoke(turret.CellOwner.coordinates.face);                   //EVENT stop timer
        }
    }
}
