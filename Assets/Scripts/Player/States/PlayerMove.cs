using UnityEngine;

public class PlayerMove : PlayerState
{
    protected Coordinates coordinates;

    public PlayerMove(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        //get previous coordinates
        this.coordinates = coordinates;
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
            coordinates = new Coordinates(face, GameManager.instance.world.worldConfig.CenterCell);

        //select cell
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.GetKeyDown(KeyCode.W))
                coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
            else if (Input.GetKeyDown(KeyCode.S))
                coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
            else if (Input.GetKeyDown(KeyCode.D))
                coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
            else if (Input.GetKeyDown(KeyCode.A))
                coordinates = WorldUtility.SelectCell(face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.left);
        }

        //save coordinates and  show selector
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
        GameManager.instance.world.Rotate(coordinates, WorldUtility.LateralFace(transform), rotateDirection);

        //change state
        player.SetState(new PlayerWaitRotation(player, coordinates));
    }
}
