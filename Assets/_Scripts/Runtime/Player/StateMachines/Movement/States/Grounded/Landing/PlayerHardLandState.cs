using RPG.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace RPG.Player.StateMachines.Movement.States.Grounded.Landing 
{
    /// <summary>
    /// Handles the player's hard land state logic.
    /// </summary>
    public class PlayerHardLandState : PlayerGroundedState
    {
        public PlayerHardLandState(PlayerStateFactory playerStateFactory) : base(playerStateFactory) { }

        #region IState Methods
        public override void Enter()
        {
            // Prevent movement by setting speed modifier to 0
            _stateFactory.ReusableData.MovementSpeedModifier = 0f;

            base.Enter(); 

            StartAnimation(_stateFactory.PlayerController.AnimationData.HardLandParameterHash);

            _stateFactory.PlayerController.Input.DisableActionFor(InputActionType.Move);

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateFactory.PlayerController.AnimationData.HardLandParameterHash);

            // Re-enable movement input after animation completes
            _stateFactory.PlayerController.Input.EnableActionFor(InputActionType.Move);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate(); 

            // If player has velocity (e.g., from physics sliding), reset to stop movement
            if (!IsMovingHorizontally()) return;

            ResetVelocity();
        }

        public override void OnAnimationExitEvent() => _stateFactory.PlayerController.Input.EnableActionFor(InputActionType.Move);

        public override void OnAnimationTransitionEvent() => _stateFactory.SwitchState(_stateFactory.IdleState);
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateFactory.PlayerController.Input.MoveStartedAction += OnMovementStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateFactory.PlayerController.Input.MoveStartedAction -= OnMovementStarted;
        }

        protected override void OnMove() => _stateFactory.SwitchState(_stateFactory.WalkState);
        #endregion

        #region Input Methods
        private void OnMovementStarted(Vector2 direction) => OnMove(); // Immediately transition to walk state

        // Ignore jump input while hard landing is playing
        protected override void OnJumpStarted() { }
        #endregion
    }
}