namespace RPG.Player.StateMachine
{
    public class PlayerMovingState : PlayerGroundedState
    {
        public PlayerMovingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.MovingParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.MovingParameterHash);
        }
        #endregion
    }
}