using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerRollState : PlayerGroundedState
    {
        public PlayerRollState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateFactory.ReusableData.MovementSpeedModifier = _groundedData.RollData.SpeedModifier;

            base.Enter();

            StartAnimation(_stateFactory.PlayerController.AnimationData.RollParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateFactory.PlayerController.AnimationData.RollParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_stateFactory.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            RotateTowardsTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            if (_stateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                _stateFactory.SwitchState(_stateFactory.IdleState);

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