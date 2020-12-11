using UnityEngine;
using UnityEngine.InputSystem;

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
        controls.Gameplay.BuildTurret.started += PressedOnBuildTurret;
        controls.Gameplay.BuildTurret.canceled += OnBuildTurret;
        controls.Gameplay.FinishStrategicPhase.started += OnPressReady;
        controls.Gameplay.FinishStrategicPhase.canceled += OnReleaseReady;
    }

    protected override void RemoveInputs()
    {
        base.RemoveInputs();

        //remove inputs
        controls.Gameplay.BuildTurret.started -= PressedOnBuildTurret;
        controls.Gameplay.BuildTurret.canceled -= OnBuildTurret;
        controls.Gameplay.FinishStrategicPhase.started -= OnPressReady;
        controls.Gameplay.FinishStrategicPhase.canceled -= OnReleaseReady;
    }

    void PressedOnBuildTurret(InputAction.CallbackContext ctx)
    {
        pressedOnBuildTurret = true;
    }

    void OnBuildTurret(InputAction.CallbackContext ctx)
    {
        //do only on click
        if (CheckClick(ref pressedOnBuildTurret) == false)
            return;

        //enter in "place turret" state
        player.SetState(new PlayerPlaceTurret(player, coordinates));
    }

    void OnPressReady(InputAction.CallbackContext ctx)
    {
        keepingPressedEndStrategic = true;
    }

    void OnReleaseReady(InputAction.CallbackContext ctx)
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
