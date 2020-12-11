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

        //stop camera movement and show preview
        StopCinemachine();
        GameManager.instance.world.Cells[coordinates].ShowPreview();
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
        controls.Gameplay.SelectCell.started += PressedSelectCell;
        controls.Gameplay.SelectCell.performed += SelectCell;
    }

    protected override void RemoveInputs()
    {
        base.RemoveInputs();

        controls.Gameplay.ConfirmTurret.started -= PressedPlaceTurret;
        controls.Gameplay.ConfirmTurret.canceled -= PlaceTurret;
        controls.Gameplay.DenyTurret.started -= PressedStopPlaceTurret;
        controls.Gameplay.DenyTurret.canceled -= StopPlaceTurret;
        controls.Gameplay.SelectCell.started -= PressedSelectCell;
        controls.Gameplay.SelectCell.performed -= SelectCell;
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

    void PressedSelectCell(InputAction.CallbackContext ctx)
    {
        pressedSelectCell = true;
    }

    void SelectCell(InputAction.CallbackContext ctx)
    {
        //do only on click
        if (CheckClick(ref pressedSelectCell) == false)
            return;

        Vector2 movement = ctx.ReadValue<Vector2>();

        //save previous coordinates
        Coordinates previousCoordinates = coordinates;

        //select cell
        if (movement.y > 0)
            coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
        else if (movement.y < 0)
            coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
        else if (movement.x > 0)
            coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
        else if (movement.x < 0)
            coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.left);

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

    #endregion
}
