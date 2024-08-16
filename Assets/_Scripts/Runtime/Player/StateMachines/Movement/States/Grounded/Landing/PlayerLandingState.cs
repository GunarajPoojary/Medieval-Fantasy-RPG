namespace RPG.Player.StateMachine
{
    public class PlayerLandingState : PlayerGroundedState
    {
        public PlayerLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.LandingParameterHash);

            DisableCameraRecentering();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.LandingParameterHash);
        }
        #endregion
    }
}