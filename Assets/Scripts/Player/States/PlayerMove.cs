using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerMove : PlayerState
{
    protected Coordinates coordinates;

    public PlayerMove(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        //get previous coordinates
        this.coordinates = coordinates;
    }

    public override void Enter()
    {
        base.Enter();

        //show selector
        GameManager.instance.uiManager.ShowSelector(coordinates);
    }

    public override void Execution()
    {
        base.Execution();

        //PROBLEMA COL MOUSE CHE DIVENTA DIPENDENTE DAL FRAME RATE
        //if moving mouse or analog, move camera
        if (controls.Gameplay.MoveCamera.phase == InputActionPhase.Started)
        {
            MoveCamera(controls.Gameplay.MoveCamera.ReadValue<Vector2>());
        }
    }

    #region inputs

    bool pressedSelectCell;
    bool pressedRotateCube;

    protected override void AddInputs()
    {
        base.AddInputs();

        //PROBLEMA CON L'ANALOGICO, SE TENGO INCLINATO VERSO DESTRA MI VEDE SOLO IL CAMBIO DI VALORE, MA POI NON RIMANE L'INPUT A DESTRA MA SI SETTA A 0
        //controls.Gameplay.MoveCamera.performed += MoveCamera;
        //controls.Gameplay.MoveCamera.canceled += MoveCamera;
        controls.Gameplay.SelectCell.started += PressedSelectCell;
        controls.Gameplay.SelectCell.performed += SelectCell;
        controls.Gameplay.RotateCube.started += PressedRotateCube;
        controls.Gameplay.RotateCube.performed += RotateCube;
    }

    protected override void RemoveInputs()
    {
        base.RemoveInputs();

        //controls.Gameplay.MoveCamera.performed -= MoveCamera;
        //controls.Gameplay.MoveCamera.canceled -= MoveCamera;
        controls.Gameplay.SelectCell.started -= PressedSelectCell;
        controls.Gameplay.SelectCell.performed -= SelectCell;
        controls.Gameplay.RotateCube.started -= PressedRotateCube;
        controls.Gameplay.RotateCube.performed -= RotateCube;
    }

    void MoveCamera(Vector2 movement)
    {        
        //move cinemachine
        CinemachineMovement(movement);

        //if change face, reselect center cell and move selector
        EFace face = WorldUtility.SelectFace(transform);

        if (face != coordinates.face)
        {
            coordinates = new Coordinates(face, GameManager.instance.world.worldConfig.CenterCell);
            GameManager.instance.uiManager.ShowSelector(coordinates);
        }
    }

    //void MoveCamera(InputAction.CallbackContext ctx)
    //{
    //    //move cinemachine
    //    CinemachineMovement(ctx.ReadValue<Vector2>());
    //
    //    //if change face, reselect center cell and move selector
    //    EFace face = WorldUtility.SelectFace(transform);
    //
    //    if (face != coordinates.face)
    //    {
    //        coordinates = new Coordinates(face, GameManager.instance.world.worldConfig.CenterCell);
    //        GameManager.instance.uiManager.ShowSelector(coordinates);
    //    }
    //}

    void PressedSelectCell(InputAction.CallbackContext ctx)
    {
        pressedSelectCell = true;
    }

    void SelectCell(InputAction.CallbackContext ctx)
    {
        //do nothing if keeping pressed, or if not clicked
        if (controls.Gameplay.KeepPressedToRotate.phase == InputActionPhase.Started || CheckClick(ref pressedSelectCell) == false)
            return;

        Vector2 movement = ctx.ReadValue<Vector2>();

        //select cell
        if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
        {
            if (movement.y > 0)
                DoSelectionCell(ERotateDirection.up);
            else if (movement.y < 0)
                DoSelectionCell(ERotateDirection.down);
        }
        else
        {
            if (movement.x > 0)
                DoSelectionCell(ERotateDirection.right);
            else if (movement.x < 0)
                DoSelectionCell(ERotateDirection.left);
        }

        //save coordinates and show selector
        GameManager.instance.uiManager.ShowSelector(coordinates);
    }

    void PressedRotateCube(InputAction.CallbackContext ctx)
    {
        pressedRotateCube = true;
    }

    void RotateCube(InputAction.CallbackContext ctx)
    {
        //do nothing if NOT keeping pressed, or if not clicked
        if (controls.Gameplay.KeepPressedToRotate.phase != InputActionPhase.Started || CheckClick(ref pressedRotateCube) == false)
            return;

        Vector2 movement = ctx.ReadValue<Vector2>();

        if (movement.y > 0)
            DoRotation(ERotateDirection.up);
        else if (movement.y < 0)
            DoRotation(ERotateDirection.down);
        else if (movement.x > 0)
            DoRotation(ERotateDirection.right);
        else if (movement.x < 0)
            DoRotation(ERotateDirection.left);
    }

    #endregion

    void DoSelectionCell(ERotateDirection direction)
    {
        //update coordinates
        coordinates = WorldUtility.SelectCell(WorldUtility.SelectFace(transform), coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), direction);
    }

    void DoRotation(ERotateDirection rotateDirection)
    {
        //if selector is greater, rotate more cells
        if (GameManager.instance.levelManager.levelConfig.SelectorSize > 1)
        {
            List<Coordinates> coordinatesToRotate = RotateMoreCells(rotateDirection);   //get list of coordinates to rotate
            coordinatesToRotate.Add(coordinates);                                       //add our coordinates
            GameManager.instance.world.Rotate(coordinatesToRotate.ToArray(), WorldUtility.LateralFace(transform), rotateDirection);
        }
        //else rotate only this cell
        else
        {
            GameManager.instance.world.Rotate(coordinates, WorldUtility.LateralFace(transform), rotateDirection);
        }

        //change state
        player.SetState(new PlayerWaitRotation(player, coordinates, WorldUtility.LateralFace(transform), rotateDirection));
    }

    List<Coordinates> RotateMoreCells(ERotateDirection rotateDirection)
    {
        int selectorSize = GameManager.instance.levelManager.levelConfig.SelectorSize;

        //check if get cells on x or y
        bool useX = rotateDirection == ERotateDirection.up || rotateDirection == ERotateDirection.down;
        int value = useX ? coordinates.x : coordinates.y;

        //check if there are enough cells to the right (useX) or up (!useX)
        bool increase = value + selectorSize - 1 < GameManager.instance.world.worldConfig.NumberCells;

        //min (next after our cell) and max (until selector size)
        //or min (from selector cell) and max (next after our cell)
        int min = increase ? value + 1 : value - (selectorSize - 1);
        int max = increase ? value + selectorSize : value;

        //increase or decrease
        List<Coordinates> coordinatesToRotate = new List<Coordinates>();
        for (int i = min; i < max; i++)
        {
            //get coordinates using x or y
            Coordinates coords = useX ? new Coordinates(coordinates.face, i, coordinates.y) : new Coordinates(coordinates.face, coordinates.x, i);

            //if there is a cell, add it
            if (GameManager.instance.world.Cells.ContainsKey(coords))
                coordinatesToRotate.Add(coords);
        }

        return coordinatesToRotate;
    }
}
