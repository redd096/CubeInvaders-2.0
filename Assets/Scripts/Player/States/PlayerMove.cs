using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerState
{
    Coordinates coordinates;
    Vector2Int selectedCell;

    public PlayerMove(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        this.coordinates = coordinates;
        selectedCell = new Vector2Int(coordinates.x, coordinates.y);
    }

    public override void Execution()
    {
        base.Execution();

        //movement
        CinemachineMovement("Mouse X", "Mouse Y");

        SelectCell();

        Rotate();
    }

    void SelectCell()
    {
        EFace face = WorldUtility.SelectFace(transform);

        //if change face, reselect center cell
        if (face != coordinates.face)
            selectedCell = GameManager.instance.world.worldConfig.CenterCell;

        //select cell
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.GetKeyDown(KeyCode.W))
                selectedCell = WorldUtility.SelectCell(face, selectedCell.x, selectedCell.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
            else if (Input.GetKeyDown(KeyCode.S))
                selectedCell = WorldUtility.SelectCell(face, selectedCell.x, selectedCell.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
            else if (Input.GetKeyDown(KeyCode.D))
                selectedCell = WorldUtility.SelectCell(face, selectedCell.x, selectedCell.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
            else if (Input.GetKeyDown(KeyCode.A))
                selectedCell = WorldUtility.SelectCell(face, selectedCell.x, selectedCell.y, WorldUtility.LateralFace(transform), ERotateDirection.left);
        }

        //show selector
        coordinates = new Coordinates(face, selectedCell.x, selectedCell.y);
        GameManager.instance.uiManager.ShowSelector(coordinates);
    }

    void Rotate()
    {
        //rotate
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.GetKeyDown(KeyCode.W))
                DoRotation(ERotateDirection.up);
            else if (Input.GetKeyDown(KeyCode.S))
                DoRotation(ERotateDirection.down);
            else if (Input.GetKeyDown(KeyCode.D))
                DoRotation(ERotateDirection.right);
            else if (Input.GetKeyDown(KeyCode.A))
                DoRotation(ERotateDirection.left);
        }
    }

    void DoRotation(ERotateDirection rotateDirection)
    {
        //do rotation
        GameManager.instance.world.Rotate(WorldUtility.SelectFace(transform), selectedCell.x, selectedCell.y, WorldUtility.LateralFace(transform), rotateDirection);

        //change state
        player.SetState(new PlayerWaitRotation(player, coordinates));
    }
}
