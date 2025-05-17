using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerHardLandState : PlayerGroundedState
    {
        public PlayerHardLandState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateFactory.ReusableData.MovementSpeedModifier = 0f;

            base.Enter();

            StartAnimation(_stateFactory.PlayerController.AnimationData.HardLandParameterHash);

            _stateFactory.PlayerController.Input.PlayerActions.Move.Disable();

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateFactory.PlayerController.AnimationData.HardLandParameterHash);

            _stateFactory.PlayerController.Input.PlayerActions.Move.Enable();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!IsMovingHorizontally())
            {
                return;
            }

            ResetVelocity();
        }

        public override void OnAnimationExitEvent() => _stateFactory.PlayerController.Input.PlayerActions.Move.Enable();

        public override void OnAnimationTransitionEvent() => _stateFactory.SwitchState(_stateFactory.IdleState);
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateFactory.PlayerController.Input.PlayerActions.Move.started += OnMovementStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateFactory.PlayerController.Input.PlayerActions.Move.started -= OnMovementStarted;
        }

        protected override void OnMove()
        {
            _stateFactory.SwitchState(_stateFactory.WalkState);
        }
        #endregion

        #region Input Methods
        private void OnMovementStarted(InputAction.CallbackContext context) => OnMove();

        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
        }
        #endregion
    }
}