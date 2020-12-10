using UnityEngine;
using System.Collections.Generic;

public class PlayerMove : PlayerState
{
    protected Coordinates coordinates;

    bool keepingPressedToRotate;

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

    #region inputs

    protected override void AddInputs()
    {
        base.AddInputs();

        controls.Gameplay.MoveCamera.performed += ctx => MoveCamera(ctx.ReadValue<Vector2>());
        controls.Gameplay.KeepPressedToRotate.started += ctx => PressedInputToRotate();
        controls.Gameplay.KeepPressedToRotate.canceled += ctx => ReleasedInputToRotate();
        controls.Gameplay.SelectCell.performed += ctx => SelectCell(ctx.ReadValue<Vector2>());
        controls.Gameplay.RotateCube.performed += ctx => RotateCube(ctx.ReadValue<Vector2>());
    }

    protected override void RemoveInputs()
    {
        base.RemoveInputs();

        controls.Gameplay.MoveCamera.performed -= ctx => MoveCamera(ctx.ReadValue<Vector2>());
        controls.Gameplay.KeepPressedToRotate.started -= ctx => PressedInputToRotate();
        controls.Gameplay.KeepPressedToRotate.canceled -= ctx => ReleasedInputToRotate();
        controls.Gameplay.SelectCell.performed -= ctx => SelectCell(ctx.ReadValue<Vector2>());
        controls.Gameplay.RotateCube.performed -= ctx => RotateCube(ctx.ReadValue<Vector2>());
    }

    void MoveCamera(Vector2 movement)
    {
        CinemachineMovement(movement);

        //if change face, reselect center cell and move selector
        EFace face = WorldUtility.SelectFace(transform);

        if (face != coordinates.face)
        {
            coordinates = new Coordinates(face, GameManager.instance.world.worldConfig.CenterCell);
            GameManager.instance.uiManager.ShowSelector(coordinates);
        }
    }

    void PressedInputToRotate()
    {
        keepingPressedToRotate = true;
    }

    void ReleasedInputToRotate()
    {
        keepingPressedToRotate = false;
    }

    void SelectCell(Vector2 movement)
    {
        //do nothing if keeping pressed
        if (keepingPressedToRotate)
            return;

        Debug.Log("select cell");
        EFace face = WorldUtility.SelectFace(transform);

        //select cell
        if (movement.y > 0)
            coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
        else if (movement.y < 0)
            coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
        else if (movement.x > 0)
            coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
        else if (movement.x < 0)
            coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.left);

        //save coordinates and show selector
        GameManager.instance.uiManager.ShowSelector(coordinates);
    }

    void RotateCube(Vector2 movement)
    {
        //do nothing if not keeping pressed
        if (keepingPressedToRotate == false)
            return;

        Debug.Log("rotate cube");

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
