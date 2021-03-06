﻿public class PlayerWaitRotation : PlayerState
{
    Coordinates coordinates;
    EFace lookingFace;
    ERotateDirection rotateDirection;

    int nRotations;

    public PlayerWaitRotation(redd096.StateMachine stateMachine, Coordinates coordinates, EFace lookingFace, ERotateDirection rotateDirection) : base(stateMachine)
    {
        this.coordinates = coordinates;
        this.lookingFace = lookingFace;
        this.rotateDirection = rotateDirection;
    }

    public override void Enter()
    {
        base.Enter();

        //wait end rotation
        GameManager.instance.world.onEndRotation += OnEndRotation;
        nRotations = 0;

        //hide selector and stop movement
        GameManager.instance.uiManager.HideSelector();
        StopCinemachine();
    }

    public override void Exit()
    {
        base.Exit();

        //remove event
        GameManager.instance.world.onEndRotation -= OnEndRotation;
    }

    void OnEndRotation()
    {
        nRotations++;

        //if must to do other rotations, do it
        if(nRotations < GameManager.instance.levelManager.levelConfig.NumberRotations)
        {
            GameManager.instance.world.Rotate(coordinates, lookingFace, rotateDirection);
        }
        //else, end every rotation
        else
        {
            OnEndEveryRotation();
        }
    }

    void OnEndEveryRotation()
    {
        //come back to movement
        if (GameManager.instance.levelManager.CurrentPhase == EPhase.strategic)
            player.SetState(new PlayerStrategic(player, coordinates));
        else
            player.SetState(new PlayerAssault(player, coordinates));
    }
}
