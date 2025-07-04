using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Class responsible for handling player idle state.
    /// </summary>
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.IdleData.SpeedModifier;

            base.Enter();

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.StationaryForce;

            ResetVelocity();

            ApplyMotion();
        }
        #endregion

        #region Input Methods
        protected override void OnMovementPerformed(Vector2 moveInput) => OnMove();
        #endregion
    }
}