using UnityEngine;

public class PlayerStrategic : PlayerMove
{
    float timeToEndStrategic;

    public PlayerStrategic(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine, coordinates)
    {
    }

    public override void Execution()
    {
        base.Execution();

        //if no click, reset slider
        if(timeToEndStrategic <= 0)
        {
            GameManager.instance.uiManager.UpdateReadySlider(0);
            return;
        }

        //else check if end
        if (Time.time > timeToEndStrategic)
        {
            EndStrategic();
        }

        //and update UI
        float remainingTime = timeToEndStrategic - Time.time;
        float timeToEnd = GameManager.instance.levelManager.generalConfig.TimeToEndStrategic;

        GameManager.instance.uiManager.UpdateReadySlider(1 - (remainingTime / timeToEnd));
    }

    #region inputs

    bool pressedOnBuildTurret;

    protected override void AddInputs()
    {
        base.AddInputs();

        //add inputs
        controls.Gameplay.BuildTurret.started += ctx => PressedOnBuildTurret();
        controls.Gameplay.BuildTurret.canceled += ctx => OnBuildTurret();
        controls.Gameplay.FinishStrategicPhase.started += ctx => OnPressReady();
        controls.Gameplay.FinishStrategicPhase.canceled += ctx => OnReleaseReady();
    }

    protected override void RemoveInputs()
    {
        base.RemoveInputs();

        //remove inputs
        controls.Gameplay.BuildTurret.started -= ctx => PressedOnBuildTurret();
        controls.Gameplay.BuildTurret.canceled -= ctx => OnBuildTurret();
        controls.Gameplay.FinishStrategicPhase.started -= ctx => OnPressReady();
        controls.Gameplay.FinishStrategicPhase.canceled -= ctx => OnReleaseReady();
    }

    void PressedOnBuildTurret()
    {
        pressedOnBuildTurret = true;
    }

    void OnBuildTurret()
    {
        //do only if pressed input
        if (pressedOnBuildTurret == false)
            return;

        pressedOnBuildTurret = false;

        //enter in "place turret" state
        player.SetState(new PlayerPlaceTurret(player, coordinates));
    }

    void OnPressReady()
    {
        //set timer
        timeToEndStrategic = Time.time + GameManager.instance.levelManager.generalConfig.TimeToEndStrategic;
    }

    void OnReleaseReady()
    {
        //reset timer
        timeToEndStrategic = 0;
    }

    #endregion

    void EndStrategic()
    {
        //end strategic phase
        GameManager.instance.levelManager.EndStrategicPhase();
    }
}
