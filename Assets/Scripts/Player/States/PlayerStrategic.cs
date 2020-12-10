using UnityEngine;

public class PlayerStrategic : PlayerMove
{
    bool keepingPressedEndStrategic;
    float timeToEndStrategic;

    public PlayerStrategic(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine, coordinates)
    {
    }

    public override void Execution()
    {
        base.Execution();

        float timeToEnd = GameManager.instance.levelManager.generalConfig.TimeToEndStrategic;

        //if keeping pressed, update slider
        if (keepingPressedEndStrategic)
        {
            timeToEndStrategic += Time.deltaTime;

            //check if end
            if (timeToEndStrategic >= timeToEnd)
            {
                EndStrategic();
            }
        }

        //update UI
        GameManager.instance.uiManager.UpdateReadySlider(timeToEndStrategic / timeToEnd);
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
        keepingPressedEndStrategic = true;
    }

    void OnReleaseReady()
    {
        keepingPressedEndStrategic = false;

        //reset slider
        timeToEndStrategic = 0;
    }

    #endregion

    void EndStrategic()
    {
        //end strategic phase
        GameManager.instance.levelManager.EndStrategicPhase();
    }
}
