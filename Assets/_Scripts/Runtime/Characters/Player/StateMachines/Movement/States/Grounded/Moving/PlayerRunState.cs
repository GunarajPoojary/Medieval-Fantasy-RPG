using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Handles the player running state logic while grounded
    /// </summary>
    public class PlayerRunState : PlayerMoveState
    {
        public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.MoveData.RunData.SpeedModifier;

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.MediumForce;

            base.Enter();
        }
        #endregion

        #region Input Methods
        protected override void OnRunCanceled()
        {
            base.OnRunCanceled();

            if (_stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                _stateMachine.SwitchState(_stateMachine.WalkState);
                return;
            }

            _stateMachine.SwitchState(_stateMachine.IdleState);
        }

        protected override void OnMovementCanceled()
        {
            base.OnMovementCanceled();

            _stateMachine.SwitchState(_stateMachine.IdleState);
        }
        #endregion
    }
}