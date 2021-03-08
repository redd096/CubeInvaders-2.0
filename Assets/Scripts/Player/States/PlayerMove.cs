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

        //enable camera movement
        player.VirtualCam.enabled = true;
    }

    public override void Execution()
    {
        base.Execution();

        //set invert Y
        player.VirtualCam.m_YAxis.m_InvertInput = player.invertY;

        //set max speed
        if (player.Controls.Gameplay.MoveCamera.activeControl != null)
        {
            //if delta (so mouse movement) don't use deltaTime
            if (player.Controls.Gameplay.MoveCamera.activeControl.name == "delta")
            {
                player.VirtualCam.m_XAxis.m_MaxSpeed = player.speedX;
                player.VirtualCam.m_YAxis.m_MaxSpeed = player.speedY;
            }
            //normally, use deltaTime
            else
            {
                player.VirtualCam.m_XAxis.m_MaxSpeed = player.speedX * Time.deltaTime;
                player.VirtualCam.m_YAxis.m_MaxSpeed = player.speedY * Time.deltaTime;
            }
        }

        //move camera
        player.VirtualCam.m_XAxis.m_InputAxisValue = player.Controls.Gameplay.MoveCamera.ReadValue<Vector2>().x;
        player.VirtualCam.m_YAxis.m_InputAxisValue = player.Controls.Gameplay.MoveCamera.ReadValue<Vector2>().y;

        //when move camera, check if changed face
        CheckChangedFace();

        //rotate cube or select cell (check if keeping pressed to rotate)
        if(controls.Gameplay.KeepPressedToRotate.phase == InputActionPhase.Started)
            RotateCube(controls.Gameplay.RotateCube.ReadValue<Vector2>());
        else
            SelectCell(controls.Gameplay.SelectCell.ReadValue<Vector2>());
    }

    public override void Exit()
    {
        base.Exit();

        //stop camera movement
        player.VirtualCam.enabled = false;
    }

    #region private API

    bool pressedSelectCell;
    bool pressedRotateCube;

    void CheckChangedFace()
    {        
        //if change face, reselect center cell and move selector
        EFace face = WorldUtility.SelectFace(transform);

        if (face != coordinates.face)
        {
            coordinates = new Coordinates(face, GameManager.instance.world.worldConfig.CenterCell);
            GameManager.instance.uiManager.ShowSelector(coordinates);
        }
    }

    void SelectCell(Vector2 movement)
    {
        //check if pressed input or moved analog
        if (movement.magnitude >= player.deadZoneAnalogs && pressedSelectCell == false)
        {
            pressedSelectCell = true;

            //check if y or x axis
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
        //reset when release input or analog
        else if (movement.magnitude < player.deadZoneAnalogs)
        {
            pressedSelectCell = false;
        }
    }

    void RotateCube(Vector2 movement)
    {
        //check if pressed input or moved analog
        if (movement.magnitude >= player.deadZoneAnalogs && pressedRotateCube == false)
        {
            pressedRotateCube = true;

            //check if y or x axis
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
            {
                if (movement.y > 0)
                    DoRotation(ERotateDirection.up);
                else if (movement.y < 0)
                    DoRotation(ERotateDirection.down);
            }
            else
            {
                if (movement.x > 0)
                    DoRotation(ERotateDirection.right);
                else if (movement.x < 0)
                    DoRotation(ERotateDirection.left);
            }
        }
        //reset when release input or analog
        else if (movement.magnitude < player.deadZoneAnalogs)
        {
            pressedRotateCube = false;
        }
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
        EFace lookingFace = WorldUtility.LateralFace(transform);

        //check if get cells on x or y
        bool useX = rotateDirection == ERotateDirection.up || rotateDirection == ERotateDirection.down;

        //if rotating up or down face, when looking from right or left, inverse useX
        if (coordinates.face == EFace.up || coordinates.face == EFace.down)
            if (lookingFace == EFace.right || lookingFace == EFace.left)
                useX = !useX;

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
