using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded.Moving
{
    /// <summary>
    /// Handles the walking state of the player while grounded
    /// </summary>
    public class PlayerWalkState : PlayerGroundedState
    {
        public PlayerWalkState(PlayerStateFactory stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            // Set movement speed modifier to walking speed
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.WalkData.SpeedModifier;

            base.Enter(); // Call base Enter (adds input listeners, starts animation, etc.)

            // Set jump force to the "weak" jump (used when walking)
            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.WeakForce;
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(Vector2 moveInput)
        {
            _stateMachine.SwitchState(_stateMachine.IdleState);

            base.OnMovementCanceled(moveInput);
        }
        #endregion
    }
}