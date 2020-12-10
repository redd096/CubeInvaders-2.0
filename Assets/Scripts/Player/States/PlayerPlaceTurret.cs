using UnityEngine;

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

        controls.Gameplay.ConfirmTurret.started += ctx => PressedPlaceTurret();
        controls.Gameplay.ConfirmTurret.canceled += ctx => PlaceTurret();
        controls.Gameplay.DenyTurret.started += ctx => PressedStopPlaceTurret();
        controls.Gameplay.DenyTurret.canceled += ctx => StopPlaceTurret();
        controls.Gameplay.SelectCell.started += ctx => PressedSelectCell();
        controls.Gameplay.SelectCell.canceled += ctx => SelectCell(ctx.ReadValue<Vector2>());
    }

    protected override void RemoveInputs()
    {
        base.RemoveInputs();

        controls.Gameplay.ConfirmTurret.started -= ctx => PressedPlaceTurret();
        controls.Gameplay.ConfirmTurret.canceled -= ctx => PlaceTurret();
        controls.Gameplay.DenyTurret.started -= ctx => PressedStopPlaceTurret();
        controls.Gameplay.DenyTurret.canceled -= ctx => StopPlaceTurret();
        controls.Gameplay.SelectCell.started -= ctx => PressedSelectCell();
        controls.Gameplay.SelectCell.canceled -= ctx => SelectCell(ctx.ReadValue<Vector2>());
    }

    void PressedPlaceTurret()
    {
        pressedPlaceTurret = true;
    }

    void PlaceTurret()
    {
        //do only if pressed input
        if (pressedPlaceTurret == false)
            return;

        pressedPlaceTurret = false;

        Debug.Log("confirm turret");
        //place turret
        GameManager.instance.world.Cells[coordinates].Interact();

        //back to strategic state
        player.SetState(new PlayerStrategic(player, coordinates));
    }

    void PressedStopPlaceTurret()
    {
        pressedStopPlaceTurret = true;
    }

    void StopPlaceTurret()
    {
        //do only if pressed input
        if (pressedStopPlaceTurret == false)
            return;

        pressedStopPlaceTurret = false;

        Debug.Log("deny turret");
        //back to strategic state
        player.SetState(new PlayerStrategic(player, coordinates));
    }

    void PressedSelectCell()
    {
        pressedSelectCell = true;
    }

    void SelectCell(Vector2 movement)
    {
        //do only if pressed input
        if (pressedSelectCell == false)
            return;

        pressedSelectCell = false;

        Debug.Log("Place Turret Select Cell");

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
