using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerHardLandingState : PlayerLandingState
    {
        public PlayerHardLandingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = 0f;

            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.HardLandParameterHash);

            _stateMachine.Player.Input.PlayerActions.Movement.Disable();

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.HardLandParameterHash);

            _stateMachine.Player.Input.PlayerActions.Movement.Enable();
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

        public override void OnAnimationExitEvent() => _stateMachine.Player.Input.PlayerActions.Movement.Enable();

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

        protected override void OnMove()
        {
            if (_stateMachine.ReusableData.ShouldWalk)
            {
                return;
            }

            _stateMachine.ChangeState(_stateMachine.RunningState);
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