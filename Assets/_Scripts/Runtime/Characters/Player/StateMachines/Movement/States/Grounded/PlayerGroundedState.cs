using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Handles grounded states.
    /// Base class for Idle and Move(Run and Walk) states.
    /// </summary>
    public class PlayerGroundedState : PlayerBaseState
    {
        private const float BLENDSNAPTHRESHOLD = 0.01f;

        public PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            _stateMachine.ReusableData.CurrentVerticalVelocity = 0f;

            StartAnimation(_stateMachine.PlayerController.AnimationData.GroundedParameterHash);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            ReadMovementInput();

            UpdateBlendTreeAnimation();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.PlayerController.AnimationData.GroundedParameterHash);
        }
        #endregion

        #region  Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateMachine.PlayerController.Input.JumpPerformedAction += OnJumpPerformed;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateMachine.PlayerController.Input.JumpPerformedAction -= OnJumpPerformed;
        }

        protected virtual void OnMove()
        {
            if (_stateMachine.ReusableData.ShouldRun)
            {
                _stateMachine.SwitchState(_stateMachine.RunState);
                return;
            }

            _stateMachine.SwitchState(_stateMachine.WalkState);
        }
        #endregion

        #region Main Methods
        private void ReadMovementInput() =>
            _stateMachine.ReusableData.MovementInput = _stateMachine.PlayerController.Input.MoveDirection;

        // Smoothly interpolates animation blend based on movement input and thresholds.
        private void UpdateBlendTreeAnimation()
        {
            // Determine target animation speed based on input
            float targetThreshold = UpdateBlendTreeThreshold();

            // Smooth blend current animation speed towards target threshold
            _stateMachine.PlayerController.AnimationData.MovementAnimationBlend =
                Mathf.Lerp(
                    _stateMachine.PlayerController.AnimationData.MovementAnimationBlend,
                    targetThreshold,
                    Time.deltaTime * _stateMachine.PlayerController.AnimationData.AnimationBlendSpeed);

            // Snap to zero if blend is too low
            if (_stateMachine.PlayerController.AnimationData.MovementAnimationBlend < BLENDSNAPTHRESHOLD)
                _stateMachine.PlayerController.AnimationData.MovementAnimationBlend = 0.0f;

            // Apply animation blend value to Animator
            _stateMachine.PlayerController.Animator.SetFloat(
                _stateMachine.PlayerController.AnimationData.SpeedParameterHash,
                _stateMachine.PlayerController.AnimationData.MovementAnimationBlend
            );
        }

        // Calculates the target threshold for animation blending based on input state.
        private float UpdateBlendTreeThreshold()
        {
            float targetThreshold;

            if (_stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                targetThreshold = _stateMachine.ReusableData.ShouldRun
                    ? _stateMachine.PlayerController.AnimationData.RunThreshold
                    : _stateMachine.PlayerController.AnimationData.WalkThreshold;
            }
            else
            {
                targetThreshold = _groundedData.IdleData.SpeedModifier;
            }

            return targetThreshold;
        }
        #endregion

        #region Input Methods
        protected virtual void OnJumpPerformed() => _stateMachine.SwitchState(_stateMachine.JumpState);
        #endregion
    }
}