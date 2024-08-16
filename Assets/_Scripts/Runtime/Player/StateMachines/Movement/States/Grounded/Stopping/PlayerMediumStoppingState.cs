namespace RPG.Player.StateMachine
{
    public class PlayerMediumStoppingState : PlayerStoppingState
    {
        public PlayerMediumStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.MediumStopParameterHash);

            _stateMachine.ReusableData.MovementDecelerationForce = _groundedData.StopData.MediumDecelerationForce;

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.MediumForce;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.MediumStopParameterHash);
        }
        #endregion
    }
}