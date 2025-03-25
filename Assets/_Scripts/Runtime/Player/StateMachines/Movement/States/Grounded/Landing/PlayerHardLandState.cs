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
            StateFactory.ReusableData.MovementSpeedModifier = 0f;

            base.Enter();

            StartAnimation(StateFactory.PlayerController.AnimationData.HardLandParameterHash);

            StateFactory.PlayerController.Input.PlayerActions.Move.Disable();

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(StateFactory.PlayerController.AnimationData.HardLandParameterHash);

            StateFactory.PlayerController.Input.PlayerActions.Move.Enable();
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

        public override void OnAnimationExitEvent() => StateFactory.PlayerController.Input.PlayerActions.Move.Enable();

        public override void OnAnimationTransitionEvent() => StateFactory.ChangeState(StateFactory.IdleState);
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            StateFactory.PlayerController.Input.PlayerActions.Move.started += OnMovementStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            StateFactory.PlayerController.Input.PlayerActions.Move.started -= OnMovementStarted;
        }

        protected override void OnMove()
        {
            StateFactory.ChangeState(StateFactory.WalkState);
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