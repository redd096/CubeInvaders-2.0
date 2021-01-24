using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPlaceTurret : PlayerState
{
    Coordinates coordinates;

    public PlayerPlaceTurret(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        this.coordinates = coordinates;
    }

    public override void Enter()
    {
        base.Enter();

        //show preview
        GameManager.instance.world.Cells[coordinates].ShowPreview();
    }

    public override void Execution()
    {
        base.Execution();

        //select cell (in execution to use dead zone analogs)
        SelectCell(controls.Gameplay.SelectCell.ReadValue<Vector2>());
    }

    public override void Exit()
    {
        base.Exit();

        //be sure to remove preview
        GameManager.instance.world.Cells[coordinates].HidePreview();
    }

    #region inputs

    bool pressedPlaceTurret;
    bool pressedStopPlaceTurret;
    bool pressedSelectCell;

    protected override void AddInputs()
    {
        base.AddInputs();

        controls.Gameplay.ConfirmTurret.started += PressedPlaceTurret;
        controls.Gameplay.ConfirmTurret.canceled += PlaceTurret;
        controls.Gameplay.DenyTurret.started += PressedStopPlaceTurret;
        controls.Gameplay.DenyTurret.canceled += StopPlaceTurret;
    }

    protected override void RemoveInputs()
    {
        base.RemoveInputs();

        controls.Gameplay.ConfirmTurret.started -= PressedPlaceTurret;
        controls.Gameplay.ConfirmTurret.canceled -= PlaceTurret;
        controls.Gameplay.DenyTurret.started -= PressedStopPlaceTurret;
        controls.Gameplay.DenyTurret.canceled -= StopPlaceTurret;
    }

    void PressedPlaceTurret(InputAction.CallbackContext ctx)
    {
        pressedPlaceTurret = true;
    }

    void PlaceTurret(InputAction.CallbackContext ctx)
    {
        //do only on click
        if (CheckClick(ref pressedPlaceTurret) == false)
            return;

        //place turret
        GameManager.instance.world.Cells[coordinates].Interact();

        //back to strategic state
        player.SetState(new PlayerStrategic(player, coordinates));
    }

    void PressedStopPlaceTurret(InputAction.CallbackContext ctx)
    {
        pressedStopPlaceTurret = true;
    }

    void StopPlaceTurret(InputAction.CallbackContext ctx)
    {
        //do only on click
        if (CheckClick(ref pressedStopPlaceTurret) == false)
            return;

        //back to strategic state
        player.SetState(new PlayerStrategic(player, coordinates));
    }

    void SelectCell(Vector2 movement)
    {
        //check if pressed input or moved analog
        if (movement.magnitude >= player.deadZoneAnalogs && pressedSelectCell == false)
        {
            pressedSelectCell = true;

            //save previous coordinates
            Coordinates previousCoordinates = coordinates;

            //select cell
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
            {
                if (movement.y > 0)
                    coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
                else if (movement.y < 0)
                    coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
            }
            else
            {
                if (movement.x > 0)
                    coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
                else if (movement.x < 0)
                    coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.left);
            }

            //if differents coordinates
            if (previousCoordinates != coordinates)
            {
                //hide old preview and show new one
                GameManager.instance.world.Cells[previousCoordinates].HidePreview();
                GameManager.instance.world.Cells[coordinates].ShowPreview();
            }

            //show selector on new coordinates
            GameManager.instance.uiManager.ShowSelector(coordinates);
        }
        //reset when release input or analog
        else if (movement.magnitude < player.deadZoneAnalogs)
        {
            pressedSelectCell = false;
        }
    }

    #endregion
}
