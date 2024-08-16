namespace RPG.Player.StateMachine
{
    public class PlayerLightStoppingState : PlayerStoppingState
    {
        public PlayerLightStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Method
        public override void Enter()
        {
            base.Enter();

            _stateMachine.ReusableData.MovementDecelerationForce = _groundedData.StopData.LightDecelerationForce;

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.WeakForce;
        }
        #endregion
    }
}