using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded.Landing 
{
    /// <summary>
    /// Handles the player's hard land state logic.
    /// </summary>
    public class PlayerHardLandState : PlayerGroundedState
    {
        public PlayerHardLandState(PlayerStateFactory stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            // Prevent movement by setting speed modifier to 0
            _stateMachine.ReusableData.MovementSpeedModifier = 0f;

            base.Enter(); 

            StartAnimation(_stateMachine.PlayerController.AnimationData.HardLandParameterHash);

            _stateMachine.PlayerController.Input.DisableActionFor(InputActionType.Move);

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.PlayerController.AnimationData.HardLandParameterHash);

            // Re-enable movement input after animation completes
            _stateMachine.PlayerController.Input.EnableActionFor(InputActionType.Move);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate(); 

            // If player has velocity (e.g., from physics sliding), reset to stop movement
            if (!IsMovingHorizontally()) return;

            ResetVelocity();
        }

        public override void OnAnimationExitEvent() => _stateMachine.PlayerController.Input.EnableActionFor(InputActionType.Move);

        public override void OnAnimationTransitionEvent() => _stateMachine.SwitchState(_stateMachine.IdleState);
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateMachine.PlayerController.Input.MoveStartedAction += OnMovementStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateMachine.PlayerController.Input.MoveStartedAction -= OnMovementStarted;
        }

        protected override void OnMove() => _stateMachine.SwitchState(_stateMachine.WalkState);
        #endregion

        #region Input Methods
        private void OnMovementStarted(Vector2 direction) => OnMove(); // Immediately transition to walk state

        // Ignore jump input while hard landing is playing
        protected override void OnJumpStarted() { }
        #endregion
    }
}