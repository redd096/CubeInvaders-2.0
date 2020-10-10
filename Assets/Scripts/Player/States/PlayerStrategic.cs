using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStrategic : PlayerMove
{
    float timeToEndStrategic;

    public PlayerStrategic(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine, coordinates)
    {
    }

#if UNITY_ANDROID

    public override void Execution()
    {
        base.Execution();

        //if is moving (not rotating) and release touch
        if(isMoving && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            //if released on the same cell already selected
            if(selectedCell == TrySelectCell())
            {
                //enter in "place turret" state
                player.SetState(new PlayerPlaceTurret(player, selectedCell.coordinates));
            }
        }

        //keep pressed to end strategic
        //TODO
        //PressReady(Input.GetKey(KeyCode.Return));
    }

#else

    public override void Execution()
    {
        base.Execution();

        //enter in "place turret" state
        if(Input.GetKeyDown(KeyCode.Space))
        {
            player.SetState(new PlayerPlaceTurret(player, coordinates));
        }

        //keep pressed to end strategic
        PressReady(Input.GetKey(KeyCode.Return));        
    }

    void PressReady(bool inputReady)
    {
        float timeToEnd = GameManager.instance.levelManager.generalConfig.TimeToEndStrategic;

        //keep pressed to end strategic
        if (inputReady)
        {
            timeToEndStrategic += Time.deltaTime / timeToEnd;

            //check if end
            if (timeToEndStrategic >= timeToEnd)
            {
                EndStrategic();
            }
        }
        else
        {
            timeToEndStrategic = 0;
        }

        //update UI
        GameManager.instance.uiManager.UpdateReadySlider(timeToEndStrategic / timeToEnd);
    }

#endif

    void EndStrategic()
    {
        //end strategic phase
        GameManager.instance.levelManager.EndStrategicPhase();
    }
}
