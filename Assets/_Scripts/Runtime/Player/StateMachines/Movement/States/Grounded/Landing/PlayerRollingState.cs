using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerRollingState : PlayerLandingState
    {
        public PlayerRollingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.RollData.SpeedModifier;

            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.RollParameterHash);

            _stateMachine.ReusableData.ShouldSprint = false;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.RollParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            RotateTowardsTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            if (_stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _stateMachine.ChangeState(_stateMachine.MediumStoppingState);

                return;
            }

            OnMove();
        }
        #endregion

        #region Input Method
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
        }
        #endregion
    }
}