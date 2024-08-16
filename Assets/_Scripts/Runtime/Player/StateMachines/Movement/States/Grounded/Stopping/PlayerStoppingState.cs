using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerStoppingState : PlayerGroundedState
    {
        public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = 0f;

            SetBaseCameraRecenteringData();

            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.StoppingParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.StoppingParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            RotateTowardsTargetRotation();

            if (!IsMovingHorizontally())
            {
                return;
            }

            DecelerateHorizontally();
        }

        public override void OnAnimationTransitionEvent() => _stateMachine.ChangeState(_stateMachine.IdlingState);
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateMachine.Player.Input.PlayerActions.Movement.started += OnMovementStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateMachine.Player.Input.PlayerActions.Movement.started -= OnMovementStarted;
        }
        #endregion

        #region Input Methods
        private void OnMovementStarted(InputAction.CallbackContext context) => OnMove();
        #endregion
    }
}