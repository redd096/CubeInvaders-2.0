public class PlayerWaitRotation : PlayerState
{
    Coordinates coordinates;

    public PlayerWaitRotation(redd096.StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        this.coordinates = coordinates;
    }

    public override void Enter()
    {
        base.Enter();

        //wait end rotation
        GameManager.instance.world.onEndRotation += OnEndRotation;

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
        //wait end rotation and come back to movement
        if (GameManager.instance.levelManager.CurrentPhase == EPhase.strategic)
            player.SetState(new PlayerStrategic(player, coordinates));
        else
            player.SetState(new PlayerAssault(player, coordinates));
    }
}
