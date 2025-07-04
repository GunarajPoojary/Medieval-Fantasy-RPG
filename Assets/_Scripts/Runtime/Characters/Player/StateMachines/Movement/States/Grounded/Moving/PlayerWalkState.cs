using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Handles the player run states logic while grounded
    /// </summary>
    public class PlayerWalkState : PlayerMoveState
    {
        public PlayerWalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.MoveData.WalkData.SpeedModifier;

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.WeakForce;

            base.Enter();
        }
        #endregion

        #region Input Methods
        protected override void OnRunPerformed()
        {
            base.OnRunPerformed();

            if (_stateMachine.ReusableData.MovementInput != Vector2.zero)
                _stateMachine.SwitchState(_stateMachine.RunState);
        }

        protected override void OnMovementCanceled()
        {
            base.OnMovementCanceled();

            _stateMachine.SwitchState(_stateMachine.IdleState);
        }
        #endregion
    }
}