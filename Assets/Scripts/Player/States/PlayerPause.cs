public class PlayerPause : PlayerState
{
    public PlayerPause(redd096.StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        StopCinemachine();
    }
}
