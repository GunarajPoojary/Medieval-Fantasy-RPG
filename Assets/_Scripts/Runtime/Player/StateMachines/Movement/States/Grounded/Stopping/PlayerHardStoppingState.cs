namespace RPG.Player.StateMachine
{
    public class PlayerHardStoppingState : PlayerStoppingState
    {
        public PlayerHardStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.HardStopParameterHash);

            _stateMachine.ReusableData.MovementDecelerationForce = _groundedData.StopData.HardDecelerationForce;

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.StrongForce;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.HardStopParameterHash);
        }
        #endregion

        #region Reusable Methods
        protected override void OnMove()
        {
            if (_stateMachine.ReusableData.ShouldWalk)
            {
                return;
            }

            _stateMachine.ChangeState(_stateMachine.RunningState);
        }
        #endregion
    }
}